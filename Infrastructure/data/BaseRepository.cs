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


    /// <summary>
    /// Inicializa una nueva instancia del repositorio base.
    /// </summary>
    /// <param name="connectionFactory">Fábrica de conexiones a la base de datos.</param>
    /// <param name="tableName">Nombre de la tabla principal.</param>
    /// <param name="viewName">Nombre de la vista asociada (opcional).</param>
    protected BaseRepository(IDbConnectionFactory connectionFactory, string tableName, string? viewName)
    {
        _connectionFactory = connectionFactory;
        _tableName = tableName;
        _viewName = viewName;
    }

    /// <summary>
    /// Convierte un valor proveniente de la base de datos al tipo de destino.
    /// </summary>
    /// <param name="value">Valor original obtenido del lector de datos.</param>
    /// <param name="targetType">Tipo de destino.</param>
    /// <returns>
    /// Valor convertido al tipo correspondiente, o <c>null</c> si el valor es nulo.
    /// </returns>
    /// <remarks>
    /// Incluye manejo especial para conversiones de valores booleanos
    /// almacenados como BIT o TINYINT.
    /// </remarks>
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

    /// <summary>
    /// Mapea una fila del <see cref="DbDataReader"/> a una instancia de la entidad.
    /// </summary>
    /// <param name="reader">Lector de datos con la fila actual.</param>
    /// <returns>Entidad mapeada.</returns>
    /// <remarks>
    /// Se ignoran propiedades marcadas con <see cref="NotMappedAttribute"/>.
    /// </remarks>
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

    /// <summary>
    /// Obtiene una entidad por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <returns>
    /// La entidad encontrada o <c>null</c> si no existe.
    /// </returns>
    public virtual async Task<T?> FindById(long id)
    {
        // Usamos el builder, agregando la condición por Id
        var result = await CreateQueryBuilder()
            .Where("id = @id", new DbParam("@id", id))
            .Limit(1)
            .ToListAsync<T>();

        // Retornamos el primer elemento si existe
        return result.FirstOrDefault();
    }

    /// <summary>
    /// Obtiene todas las entidades de la tabla asociada.
    /// </summary>
    /// <returns>Lista completa de entidades.</returns>
    public virtual async Task<List<T>> FindAll()
    {
        var result = await CreateQueryBuilder()
            .ToListAsync<T>();

        return result;
    }

    /// <summary>
    /// Ejecuta una consulta SELECT dinámica sobre una tabla o vista,
    /// aplicando una cláusula WHERE obligatoria, ordenamiento opcional
    /// y paginación mediante LIMIT/OFFSET.
    /// 
    /// Este método es genérico y no contiene reglas de negocio.
    /// Las decisiones sobre filtros, ordenamiento y paginación
    /// deben tomarse en capas superiores (servicios/repositorios concretos).
    /// </summary>
    /// <param name="tableOrView">
    /// Nombre de la tabla o vista sobre la que se ejecutará la consulta.
    /// </param>
    /// <param name="where">
    /// Condición WHERE sin incluir la palabra clave WHERE.
    /// </param>
    /// <param name="orderBy">
    /// Expresión ORDER BY sin incluir la palabra clave ORDER BY.
    /// Ejemplo: "fecha DESC".
    /// </param>
    /// <param name="limit">
    /// Cantidad máxima de registros a devolver. Si es null, no se aplica límite.
    /// </param>
    /// <param name="offset">
    /// Cantidad de registros a omitir (usado para paginación).
    /// Solo se aplica si <paramref name="limit"/> tiene valor.
    /// </param>
    /// <param name="parameters">
    /// Parámetros utilizados en la cláusula WHERE.
    /// </param>
    /// <returns>
    /// Lista de entidades mapeadas que cumplen la condición especificada.
    /// </returns>
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
    /// <summary>
    /// Elimina una entidad por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <returns>
    /// <c>true</c> si la entidad fue eliminada; de lo contrario, <c>false</c>.
    /// </returns>
    public async Task<bool> DeleteById(long id)
    {
        int affected = await ExecuteNonQueryAsync(
            $"DELETE FROM {_tableName} WHERE id = @id",
            [new DbParam("@id", id)]
        );

        return affected > 0;
    }

    /// <summary>
    /// Actualiza una entidad existente en la base de datos.
    /// </summary>
    /// <param name="entity">Entidad a actualizar.</param>
    /// <returns>
    /// <c>true</c> si la actualización fue exitosa.
    /// </returns>
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

    /// <summary>
    /// Guarda una nueva entidad en la base de datos.
    /// </summary>
    /// <param name="entity">Entidad a persistir.</param>
    /// <returns>
    /// <c>true</c> si la entidad fue guardada correctamente.
    /// </returns>
    /// <remarks>
    /// El identificador generado se asigna automáticamente a la entidad.
    /// </remarks>
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

    /// <summary>
    /// Cuenta la cantidad de registros que cumplen una condición.
    /// </summary>
    /// <param name="where">Condición WHERE (sin la palabra WHERE).</param>
    /// <param name="parameters">Parámetros de la consulta.</param>
    /// <returns>Número total de registros.</returns>
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

    /// <summary>
    /// Obtiene todas las entidades asociadas a una empresa.
    /// </summary>
    /// <param name="empresaId">Identificador de la empresa.</param>
    /// <returns>Lista de entidades asociadas a la empresa.</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si no existe una vista configurada.
    /// </exception>
    public virtual async Task<List<T>> FindAllByEmpresa(long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        var builder = new QueryBuilder<T>(this)
            .Where("empresa = @empresa", new DbParam("@empresa", empresaId))
            .OrderBy("COALESCE(fecha, '1900-01-01') DESC");

        return await builder.ToListAsync<T>();
    }

    /// <summary>
    /// Obtiene una página de entidades asociadas a una empresa.
    /// </summary>
    /// <param name="empresaId">Identificador de la empresa.</param>
    /// <param name="pageNumber">Número de página (comenzando en 1).</param>
    /// <param name="pageSize">Cantidad de registros por página.</param>
    /// <returns>Lista paginada de entidades.</returns>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si no existe una vista configurada.
    /// </exception>
    public virtual async Task<List<T>> FindPageByEmpresa(long empresaId, int pageNumber, int pageSize)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        var builder = new QueryBuilder<T>(this)
            .Where("empresa = @empresa", new DbParam("@empresa", empresaId))
            .OrderBy("COALESCE(fecha, '1900-01-01') DESC")
            .Limit(pageSize, (pageNumber - 1) * pageSize);

        return await builder.ToListAsync<T>();
    }

    public async Task<long> ContarPorEmpresa(long empresaId)
    {
        long total = await CreateQueryBuilder()
            .Where("empresa = @empresa", new DbParam("@empresa", empresaId))
            .CountAsync();

        return total;
    }  

    public virtual async Task<List<T>> FindPageWhere(
        string where,
        string? orderBy,
        int pageNumber,
        int pageSize,
        IEnumerable<DbParam>? parameters = null)
    {
        if (_viewName == null)
            throw new InvalidOperationException(
                "La vista no está asignada para este repositorio.");

        int offset = (pageNumber - 1) * pageSize;

        return await FindWhereFrom(
            tableOrView: _viewName,
            where: where,
            orderBy: orderBy,
            limit: pageSize,
            offset: offset,
            parameters: parameters);
    }

    public async Task<bool> ExistsByColumns(
        Dictionary<string, object> columns, 
        long? excludeId = null)
    {
        var builder = CreateQueryBuilder();

        foreach (var kv in columns)
            builder.Where($"{kv.Key} = @{kv.Key}", new DbParam($"@{kv.Key}", kv.Value));

        if (excludeId.HasValue)
            builder.Where("id <> @excludeId", new DbParam("@excludeId", excludeId.Value));

        // Usamos Select("1") para que CountAsync haga SELECT 1 y no devuelva objetos completos
        long count = await builder.Select("1").CountAsync();

        return count > 0;
    }

    public async Task<List<TData>> GetColumnList<TData>(string columnName, string where, IEnumerable<DbParam>? parameters = null)
    {
        var builder = CreateQueryBuilder()
            .Select(columnName)
            .Where(where, parameters?.ToArray() ?? []);

        return await builder.ToListAsync<TData>();
    }
}