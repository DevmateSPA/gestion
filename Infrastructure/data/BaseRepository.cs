using System.Data.Common;
using System.Reflection;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.model;
using Gestion.Infrastructure.Services;
using Gestion.core.interfaces.database;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;
using System.Text;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;


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
    protected readonly string _tableName;

    /// <summary>
    /// Nombre de la vista asociada a la entidad, si aplica.
    /// </summary>
    protected readonly string? _viewName;

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
    protected object? ConvertValue(object? value, Type targetType)
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

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            if (!prop.CanWrite)
                continue;

            if (Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                continue;

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
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {_tableName} WHERE id = @id";

        var param = cmd.CreateParameter();
        param.ParameterName = "@id";
        param.Value = id;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapEntity(reader: reader);

        return default;
    }

    /// <summary>
    /// Obtiene todas las entidades de la tabla asociada.
    /// </summary>
    /// <returns>Lista completa de entidades.</returns>
    public virtual async Task<List<T>> FindAll()
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {_tableName}";

        using var reader = await cmd.ExecuteReaderAsync();

        var entities = new List<T>();
        while (await reader.ReadAsync())
        {
            entities.Add(MapEntity(reader: reader));
        }

        return entities;
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
        params DbParameter[] parameters)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        var sql = new StringBuilder($"SELECT * FROM {tableOrView} WHERE {where}");

        if (!string.IsNullOrWhiteSpace(orderBy))
            sql.Append($" ORDER BY {orderBy}");

        // Agregar LIMIT y OFFSET si existen
        if (limit.HasValue)
        {
            sql.Append(" LIMIT @limit");

            var limitParam = cmd.CreateParameter();
            limitParam.ParameterName = "@limit";
            limitParam.Value = limit.Value;
            cmd.Parameters.Add(limitParam);

            if (offset.HasValue)
            {
                sql.Append(" OFFSET @offset");

                var offsetParam = cmd.CreateParameter();
                offsetParam.ParameterName = "@offset";
                offsetParam.Value = offset.Value;
                cmd.Parameters.Add(offsetParam);
            }
        }

        cmd.CommandText = sql.ToString();

        // Agregar los parámetros del WHERE
        foreach (var p in parameters)
            cmd.Parameters.Add(p);

        using var reader = await cmd.ExecuteReaderAsync();

        var list = new List<T>();
        while (await reader.ReadAsync())
            list.Add(MapEntity(reader: reader));

        return list;
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
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
        cmd.CommandText = $"DELETE FROM {_tableName} WHERE id = @id";

        var param = cmd.CreateParameter();
        param.ParameterName = "@id";
        param.Value = id;
        cmd.Parameters.Add(param);

        int affected = await cmd.ExecuteNonQueryAsync();
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
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        var props = typeof(T).GetProperties()
            .Where(p => p.Name != "Id" 
            && Attribute.IsDefined(p, typeof(NotMappedAttribute)) == false
            && !Attribute.IsDefined(p, typeof(DbIgnoreAttribute)))
            .ToList();

        var setClause = string.Join(", ", props.Select(p => $"{p.Name.ToLower()} = @{p.Name.ToLower()}"));

        cmd.CommandText = $"UPDATE {_tableName} SET {setClause} WHERE id = @id";

        foreach (var prop in props)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = $"@{prop.Name.ToLower()}";
            param.Value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }

        var idParam = cmd.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = typeof(T).GetProperty("Id")?.GetValue(entity) ?? DBNull.Value;
        cmd.Parameters.Add(idParam);

        int affected = await cmd.ExecuteNonQueryAsync();
        return affected > 0;
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
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        var props = typeof(T).GetProperties()
            .Where(p => p.Name != "Id"
            && Attribute.IsDefined(p, typeof(NotMappedAttribute)) == false
            && !Attribute.IsDefined(p, typeof(DbIgnoreAttribute)))
            .ToList();

        var columns = string.Join(", ", props.Select(p => p.Name.ToLower()));
        var parameters = string.Join(", ", props.Select(p => $"@{p.Name.ToLower()}"));

        cmd.CommandText = $@"
            INSERT INTO {_tableName} ({columns}) VALUES ({parameters});
            SELECT LAST_INSERT_ID();
        ";

        foreach (var prop in props)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = $"@{prop.Name.ToLower()}";
            param.Value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }

        var result = await cmd.ExecuteScalarAsync();

        if (result == null || result == DBNull.Value)
            return false;

        // Asignar el ID al objeto
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
    public async Task<long> CountWhere(string where, params DbParameter[] parameters)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = $"SELECT COUNT(1) FROM {_tableName} WHERE {where}";

        foreach (var p in parameters)
            cmd.Parameters.Add(p);

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

        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId)
        ];

        return await FindWhereFrom(
            tableOrView: _viewName,
            where: "empresa = @empresa",
            orderBy: null,
            limit: null,
            offset: null,
            parameters: parameters);
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

        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId)
        ];

        return await FindPageWhere(
            where: "empresa = @empresa",
            orderBy: null,
            pageNumber: pageNumber,
            pageSize: pageSize,
            parameters: parameters);
    }

    public virtual async Task<long> ContarPorEmpresa(long empresaId)
    {
        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId)
        ];

        return await CountWhere(where: "empresa = @empresa",
        parameters: parameters);
    }

    public virtual async Task<List<T>> FindPageWhere(
        string where,
        string? orderBy,
        int pageNumber,
        int pageSize,
        params DbParameter[] parameters)
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
}