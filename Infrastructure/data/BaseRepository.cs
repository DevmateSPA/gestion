using System.Data.Common;
using System.Reflection;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.model;
using Gestion.Infrastructure.Services;
using Gestion.core.interfaces.database;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using System.Text;
using Gestion.Infrastructure.Logging;
using System.Data;
using System.Collections.Concurrent;

namespace Gestion.Infrastructure.data;

public record DbParam(string Name, object? Value);


/// <summary>
/// Repositorio base genérico que proporciona operaciones CRUD y consultas comunes
/// para entidades persistentes.
/// </summary>
/// <typeparam name="T">
/// Tipo de entidad que implementa <see cref="IModel"/> y posee constructor sin parámetros.
/// </typeparam>
/// <remarks>
/// Este repositorio utiliza reflexión para mapear dinámicamente las columnas
/// devueltas por la base de datos a las propiedades de la entidad.
/// </remarks>
public abstract class BaseRepository<T> : IBaseRepository<T> where T : IModel, new()

{
    /// <summary>
    /// Fábrica de conexiones a la base de datos.
    /// </summary>
    protected readonly IDbConnectionFactory _connectionFactory;

    /// <summary>
    /// Nombre de la tabla asociada a la entidad.
    /// </summary>
    public readonly string _tableName;

    /// <summary>
    /// Nombre de la vista asociada a la entidad, si aplica.
    /// </summary>
    public readonly string? _viewName;

    // Cache estático para todas las entidades
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _propertyCache
        = new();


    protected BaseRepository(IDbConnectionFactory connectionFactory, string tableName, string? viewName)
    {
        _connectionFactory = connectionFactory;
        _tableName = tableName;
        _viewName = viewName;
    }
    public object? ConvertValue(object? value, Type targetType)
    {
        if (value == null || value == DBNull.Value)
            return null;

        // --- Conversión especial BIT/TINYINT → bool ---
        if (targetType == typeof(bool))
        {
            return value switch
            {
                bool b => b,
                byte bt => bt == 1,
                sbyte sb => sb == 1,
                byte[] arr when arr.Length > 0 => arr[0] == 1,
                _ => false
            };
        }
        // ------------------------------------------------

        // Conversión genérica para otros tipos
        if (targetType.IsEnum)
            return Enum.ToObject(targetType, value);

        return Convert.ChangeType(value, targetType);
    }

    protected DbCommand CreateCommand(
        DbConnection conn,
        string sql,
        IEnumerable<DbParam>? parameters = null)
    {
        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        if (parameters != null)
        {
            foreach (var p in parameters)
                cmd.Parameters.Add(CreateParam(cmd, p.Name, p.Value));
        }

        return cmd;
    }

    protected DbParameter CreateParam(
        DbCommand cmd,
        string name,
        object? value,
        DbType? type = null)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value ?? DBNull.Value;

        if (type.HasValue)
            p.DbType = type.Value;

        return p;
    }

    protected async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<DbParam>? parameters = null)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = CreateCommand(conn, sql, parameters);
        return await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> ExecuteNonQueryWithLogAsync(string operation, string sql, IEnumerable<DbParam>? parameters = null)
    {
        return await SqlLogger.LogAsync(
            operation: operation,
            sql: sql,
            action: async () => await ExecuteNonQueryAsync(sql, parameters),
            countSelector: result => (int)result
        );
    }

    protected QueryBuilder<T> CreateQueryBuilder()
    {
        return new QueryBuilder<T>(this);
    }

    protected virtual T MapEntity(DbDataReader reader)
    {
        var entity = new T();

        // Cache de propiedades por tipo
        var type = typeof(T);
        if (!_propertyCache.TryGetValue(type, out var props))
        {
            props = [.. type.GetProperties().Where(p => p.CanWrite && !Attribute.IsDefined(p, typeof(NotMappedAttribute)))];

            _propertyCache[type] = props;
        }

        foreach (var prop in props)
        {
            string col = prop.Name.ToLower();

            if (!reader.HasColumn(col))
                continue;

            object? rawValue = reader[col];
            object? converted = ConvertValue(rawValue, prop.PropertyType);

            prop.SetValue(entity, converted);
        }

        return entity;
    }

    public virtual async Task<T?> FindById(long id)
    {
        // Usamos el builder, agregando la condición por Id
        var result = await CreateQueryBuilder()
            .WhereEqual("id", id)
            .Limit(1)
            .ToListAsync<T>();

        // Retornamos el primer elemento si existe
        return result.FirstOrDefault();
    }

    public virtual async Task<List<T>> FindAll()
    {
        var result = await CreateQueryBuilder()
            .ToListAsync<T>();

        return result;
    }


    /// <summary>
    /// Ejecuta una consulta SELECT genérica sobre una tabla o vista específica,
    /// permitiendo definir dinámicamente:
    /// - La fuente de datos (tabla o vista)
    /// - La cláusula WHERE
    /// - Ordenamiento
    /// - Paginación (LIMIT / OFFSET)
    /// - Columnas seleccionadas
    ///
    /// Este método actúa como el núcleo de las consultas de lectura del repositorio,
    /// siendo utilizado por QueryBuilder y por consultas personalizadas que no
    /// requieren una entidad completa o un repositorio dedicado.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de entidad o DTO a mapear desde el DbDataReader.
    /// Debe ser compatible con el método MapEntity.
    /// </typeparam>
    /// <param name="tableOrView">
    /// Nombre de la tabla o vista desde la cual se realizará la consulta.
    /// Se asume que el nombre es seguro y controlado por la capa de infraestructura.
    /// </param>
    /// <param name="where">
    /// Cláusula WHERE de la consulta, sin incluir la palabra clave WHERE.
    /// Debe usar parámetros para evitar inyección SQL.
    /// </param>
    /// <param name="orderBy">
    /// Cláusula ORDER BY opcional, sin incluir la palabra clave ORDER BY.
    /// </param>
    /// <param name="limit">
    /// Cantidad máxima de registros a retornar (LIMIT).
    /// </param>
    /// <param name="offset">
    /// Cantidad de registros a omitir antes de comenzar a retornar resultados (OFFSET).
    /// Solo se aplica si limit tiene valor.
    /// </param>
    /// <param name="parameters">
    /// Colección de parámetros SQL utilizados en la cláusula WHERE.
    /// Los parámetros de paginación (LIMIT / OFFSET) se agregan automáticamente.
    /// </param>
    /// <param name="selectColumns">
    /// Columnas a seleccionar en la consulta.
    /// Por defecto selecciona todas las columnas (*).
    /// </param>
    /// <returns>
    /// Lista de entidades o DTOs mapeados desde el resultado de la consulta.
    /// </returns>
    /// <remarks>
    /// Seguridad:
    /// - tableOrView, where, orderBy y selectColumns NO deben contener input del usuario.
    /// - Este método asume que dichos valores son definidos exclusivamente por código.
    ///
    /// Diseño:
    /// - Centraliza la ejecución de consultas SELECT.
    /// - Evita duplicación de lógica de acceso a datos.
    /// - Mantiene el control del SQL en la capa de infraestructura.
    /// </remarks>
    public async Task<List<T>> FindWhereFrom(
        string tableOrView,
        string where,
        string? orderBy = null,
        int? limit = null,
        int? offset = null,
        IEnumerable<DbParam>? parameters = null,
        string selectColumns = "*")
    {
        using var conn = await _connectionFactory.CreateConnection();

        // Construimos la consulta
        var sql = new StringBuilder($"SELECT {selectColumns} FROM {tableOrView} WHERE {where}");

        if (!string.IsNullOrWhiteSpace(orderBy))
            sql.Append($" ORDER BY {orderBy}");

        if (limit.HasValue)
        {
            sql.Append(" LIMIT @limit");
            if (offset.HasValue)
                sql.Append(" OFFSET @offset");
        }

        // Creamos los parámetros, empezando por limit y offset
        var dbParams = new List<DbParam>();
        if (limit.HasValue)
            dbParams.Add(new DbParam("@limit", limit.Value));
        if (offset.HasValue)
            dbParams.Add(new DbParam("@offset", offset.Value));

        if (parameters != null)
            dbParams.AddRange(parameters);

        // Creamos el comando usando CreateCommand
        using var cmd = CreateCommand(
            conn,
            sql.ToString(),
            dbParams
        );

        return await SqlLogger.LogAsync(
            operation: typeof(T).Name,
            sql: cmd.CommandText,
            action: async () =>
            {
                using var reader = await cmd.ExecuteReaderAsync();
                var list = new List<T>();
                while (await reader.ReadAsync())
                    list.Add(MapEntity(reader));
                return list;
            },
            countSelector: list => list.Count
        );
    }

    public async Task<bool> DeleteById(long id)
    {
        int affected = await ExecuteNonQueryAsync(
            $"DELETE FROM {_tableName} WHERE id = @id",
            [new DbParam("@id", id)]
        );

        return affected > 0;
    }

    public virtual async Task<bool> Update(T entity)
    {
        var props = typeof(T).GetProperties()
            .Where(p => p.Name != "Id" 
                && Attribute.IsDefined(p, typeof(NotMappedAttribute)) == false
                && !Attribute.IsDefined(p, typeof(NoSaveDbAttribute)))
            .ToList();

        var setClause = string.Join(", ", props.Select(p => $"{p.Name.ToLower()} = @{p.Name.ToLower()}"));

        var parameters = props.Select(p => 
            new DbParam($"@{p.Name.ToLower()}", p.GetValue(entity) ?? DBNull.Value)).ToList();

        parameters.Add(new DbParam("@id", typeof(T).GetProperty("Id")?.GetValue(entity)));

        int affected = await ExecuteNonQueryAsync(
            $"UPDATE {_tableName} SET {setClause} WHERE id = @id", 
            parameters);

        return affected > 0;
    }

    protected List<DbParam> GetParametersFromEntity(T entity, bool includeId = false)
    {
        var props = typeof(T).GetProperties()
            .Where(p => (includeId || p.Name != "Id") 
                        && !Attribute.IsDefined(p, typeof(NotMappedAttribute))
                        && !Attribute.IsDefined(p, typeof(NoSaveDbAttribute)))
            .ToList();

        return props.Select(p => new DbParam($"@{p.Name.ToLower()}", p.GetValue(entity) ?? DBNull.Value)).ToList();
    }

    public virtual async Task<bool> Save(T entity)
    {
        var parametersList = GetParametersFromEntity(entity);

        var columns = string.Join(", ", parametersList.Select(p => p.Name.Substring(1))); // quitar @
        var paramNames = string.Join(", ", parametersList.Select(p => p.Name));

        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = CreateCommand(
            conn,
            $@"
            INSERT INTO {_tableName} ({columns}) VALUES ({paramNames});
            SELECT LAST_INSERT_ID();",
            parametersList
        );

        var result = await cmd.ExecuteScalarAsync();
        if (result == null || result == DBNull.Value)
            return false;

        long id = Convert.ToInt64(result);
        var propId = typeof(T).GetProperty("Id");
        if (propId != null && propId.CanWrite)
            propId.SetValue(entity, id);

        return true;
    }

    public async Task<long> CountWhere(
        string where, 
        string? tableName = null, 
        IEnumerable<DbParam>? parameters = null)
    {
        using var conn = await _connectionFactory.CreateConnection();
        tableName ??= _tableName;
        using var cmd = CreateCommand(
            conn,
            $"SELECT COUNT(1) FROM {tableName} WHERE {where}",
            parameters
        );

        var result = await cmd.ExecuteScalarAsync();

        return Convert.ToInt64(result);
    }
    public virtual async Task<List<T>> FindAllByEmpresa(long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        var builder = new QueryBuilder<T>(this)
            .WhereEqual("empresa", empresaId)
            .OrderBy("COALESCE(fecha, '1900-01-01') DESC");

        return await builder.ToListAsync<T>();
    }

    public virtual async Task<List<T>> FindPageByEmpresa(long empresaId, int pageNumber, int pageSize)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        var builder = new QueryBuilder<T>(this)
            .WhereEqual("empresa", empresaId)
            .OrderBy("COALESCE(fecha, '1900-01-01') DESC")
            .Page(pageNumber, pageSize);

        return await builder.ToListAsync<T>();
    }

    public async Task<long> ContarPorEmpresa(long empresaId)
    {
        long total = await CreateQueryBuilder()
            .WhereEqual("empresa", empresaId)
            .CountAsync();

        return total;
    }  

    public async Task<bool> Exists(
        Action<QueryBuilder<T>> build,
        long? excludeId = null)
    {
        var builder = CreateQueryBuilder();

        build(builder);

        if (excludeId.HasValue)
            builder.Where("id <> @excludeId",
                new DbParam("@excludeId", excludeId.Value));

        return await builder.CountAsync() > 0;
    }

    public async Task<bool> ExistsByColumns(
        IEnumerable<(string column, object value)> columns,
        long? excludeId = null)
    {
        return await Exists(builder =>
        {
            foreach (var (column, value) in columns)
            {
                builder.Where(
                    $"{column} = @{column}",
                    new DbParam($"@{column}", value));
            }
        }, excludeId);
    }

    public async Task<List<TData>> GetColumnList<TData>(string columnName, string where, IEnumerable<DbParam>? parameters = null)
    {
        var builder = CreateQueryBuilder()
            .Select(columnName)
            .Where(where, parameters?.ToArray() ?? []);

        return await builder.ToListAsync<TData>();
    }
}