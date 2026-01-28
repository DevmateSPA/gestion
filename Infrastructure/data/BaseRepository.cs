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

/// <summary>
/// Representa un parámetro SQL tipado utilizado en la construcción
/// y ejecución de consultas parametrizadas.
/// </summary>
/// <param name="Name">
/// Nombre del parámetro SQL (incluyendo el prefijo '@').
/// </param>
/// <param name="Value">
/// Valor asociado al parámetro. Puede ser null.
/// </param>
/// <remarks>
/// Este record no crea parámetros de base de datos por sí mismo.
/// Su propósito es transportar la información necesaria para que
/// la capa de infraestructura (BaseRepository / CreateCommand)
/// instancie y asigne los parámetros reales al DbCommand.
/// </remarks>
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

    /// <summary>
    /// Convierte un valor proveniente de la base de datos al tipo
    /// de destino especificado.
    /// </summary>
    /// <param name="value">
    /// Valor bruto obtenido desde <see cref="DbDataReader"/>.
    /// </param>
    /// <param name="targetType">
    /// Tipo de destino al que se desea convertir el valor.
    /// </param>
    /// <remarks>
    /// Este método centraliza la lógica de conversión para el mapeo
    /// de entidades, manejando:
    /// 
    /// - Valores nulos y <see cref="DBNull"/>
    /// - Conversión especial de columnas BIT/TINYINT a <see cref="bool"/>
    ///   (común en MySQL y MariaDB)
    /// - Conversión de valores enumerados
    /// - Conversión genérica mediante <see cref="Convert.ChangeType"/>
    /// 
    /// Cualquier conversión específica de proveedor debe resolverse
    /// en este método para mantener el mapeo consistente.
    /// </remarks>
    /// <returns>
    /// Valor convertido al tipo de destino, o <c>null</c> si el valor
    /// original es nulo.
    /// </returns>
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

    /// <summary>
    /// Crea y configura un <see cref="DbCommand"/> a partir de una conexión,
    /// una consulta SQL y una colección opcional de parámetros.
    /// </summary>
    /// <param name="conn">
    /// Conexión abierta a la base de datos sobre la cual se creará el comando.
    /// </param>
    /// <param name="sql">
    /// Consulta SQL a ejecutar. Debe utilizar parámetros nombrados
    /// compatibles con <see cref="DbParam"/>.
    /// </param>
    /// <param name="parameters">
    /// Colección opcional de parámetros lógicos (<see cref="DbParam"/>)
    /// que serán transformados en parámetros reales del proveedor
    /// y asociados al comando.
    /// </param>
    /// <remarks>
    /// Este método actúa como punto central de materialización de comandos SQL.
    /// 
    /// Responsabilidades:
    /// - Crear el <see cref="DbCommand"/> asociado a la conexión
    /// - Asignar el texto SQL
    /// - Traducir <see cref="DbParam"/> a parámetros reales del proveedor
    /// 
    /// No ejecuta el comando ni gestiona el ciclo de vida de la conexión.
    /// </remarks>
    /// <returns>
    /// Un <see cref="DbCommand"/> completamente configurado y listo para ejecutar.
    /// </returns>
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

    /// <summary>
    /// Crea un <see cref="DbParameter"/> compatible con el proveedor
    /// asociado al <see cref="DbCommand"/> recibido.
    /// </summary>
    /// <param name="cmd">
    /// Comando al cual pertenecerá el parámetro. Se utiliza para crear
    /// el parámetro específico del proveedor (MySQL, SQL Server, etc.).
    /// </param>
    /// <param name="name">
    /// Nombre del parámetro SQL (incluyendo el prefijo '@').
    /// </param>
    /// <param name="value">
    /// Valor del parámetro. Si es null, se convierte automáticamente
    /// en <see cref="DBNull.Value"/>.
    /// </param>
    /// <param name="type">
    /// Tipo de dato opcional del parámetro. Si se especifica,
    /// se asigna explícitamente al parámetro.
    /// </param>
    /// <remarks>
    /// Este método encapsula la creación de parámetros dependientes
    /// del proveedor, garantizando:
    /// - Correcta conversión de null a <see cref="DBNull.Value"/>
    /// - Compatibilidad con distintos motores de base de datos
    /// - Separación entre definición lógica y materialización física
    /// </remarks>
    /// <returns>
    /// Un <see cref="DbParameter"/> completamente configurado,
    /// listo para ser agregado al comando.
    /// </returns>
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

    /// <summary>
    /// Ejecuta una sentencia SQL que no retorna resultados
    /// (INSERT, UPDATE, DELETE, etc.).
    /// </summary>
    /// <param name="sql">
    /// Consulta SQL a ejecutar. Debe utilizar parámetros nombrados
    /// compatibles con <see cref="DbParam"/>.
    /// </param>
    /// <param name="parameters">
    /// Colección opcional de parámetros lógicos que serán
    /// materializados y asociados al comando.
    /// </param>
    /// <remarks>
    /// Este método:
    /// - Crea y gestiona la conexión a la base de datos
    /// - Construye el comando SQL mediante <see cref="CreateCommand"/>
    /// - Ejecuta la operación de forma asíncrona
    /// 
    /// No realiza logging ni manejo de transacciones.
    /// </remarks>
    /// <returns>
    /// Número de filas afectadas por la operación.
    /// </returns>
    protected async Task<int> ExecuteNonQueryAsync(string sql, IEnumerable<DbParam>? parameters = null)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = CreateCommand(conn, sql, parameters);
        return await cmd.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Ejecuta una sentencia SQL que no retorna resultados y
    /// registra la operación mediante <see cref="SqlLogger"/>.
    /// </summary>
    /// <param name="operation">
    /// Nombre lógico de la operación, utilizado para fines de logging
    /// y trazabilidad.
    /// </param>
    /// <param name="sql">
    /// Consulta SQL a ejecutar.
    /// </param>
    /// <param name="parameters">
    /// Colección opcional de parámetros lógicos asociados a la consulta.
    /// </param>
    /// <remarks>
    /// Este método envuelve <see cref="ExecuteNonQueryAsync"/> para
    /// añadir observabilidad sin duplicar lógica de acceso a datos.
    /// 
    /// El resultado logueado corresponde al número de filas afectadas.
    /// </remarks>
    /// <returns>
    /// Número de filas afectadas por la operación.
    /// </returns>
    public async Task<int> ExecuteNonQueryWithLogAsync(string operation, string sql, IEnumerable<DbParam>? parameters = null)
    {
        return await SqlLogger.LogAsync(
            operation: operation,
            sql: sql,
            action: async () => await ExecuteNonQueryAsync(sql, parameters),
            countSelector: result => result
        );
    }

    /// <summary>
    /// Crea una nueva instancia de <see cref="QueryBuilder{T}"/> asociada
    /// a este repositorio.
    /// </summary>
    /// <remarks>
    /// El <see cref="QueryBuilder{T}"/> utiliza este repositorio como
    /// punto de acceso a la infraestructura de datos, delegando en él:
    /// - La ejecución de la consulta
    /// - La creación de comandos y parámetros
    /// - El mapeo de resultados
    /// 
    /// Cada llamada retorna una instancia nueva, garantizando que
    /// el estado del builder no sea compartido entre consultas.
    /// </remarks>
    protected QueryBuilder<T> CreateQueryBuilder()
    {
        return new QueryBuilder<T>(this);
    }

    /// <summary>
    /// Mapea una fila del <see cref="DbDataReader"/> a una instancia
    /// del tipo de entidad <typeparamref name="T"/>.
    /// </summary>
    /// <param name="reader">
    /// Lector de datos posicionado en una fila válida.
    /// </param>
    /// <remarks>
    /// El mapeo se realiza por convención:
    /// - El nombre de la columna debe coincidir con el nombre de la propiedad
    ///   (ignorando mayúsculas/minúsculas).
    /// - Las propiedades marcadas con <see cref="NotMappedAttribute"/>
    ///   son ignoradas.
    /// 
    /// Para mejorar el rendimiento, las propiedades mapeables se
    /// almacenan en caché por tipo.
    /// 
    /// La conversión de tipos se delega a <see cref="ConvertValue"/>,
    /// permitiendo manejar valores nulos y conversiones seguras.
    /// </remarks>
    /// <returns>
    /// Instancia de <typeparamref name="T"/> poblada con los valores
    /// de la fila actual.
    /// </returns>
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

    /// <summary>
    /// Genera una lista de parámetros SQL a partir de las propiedades
    /// de una entidad.
    /// </summary>
    /// <param name="entity">
    /// Entidad de la cual se extraerán los valores.
    /// </param>
    /// <param name="includeId">
    /// Indica si la propiedad <c>Id</c> debe incluirse en la lista
    /// de parámetros (útil para operaciones UPDATE).
    /// </param>
    /// <remarks>
    /// La generación de parámetros se realiza por convención:
    /// 
    /// - Cada propiedad pública y escribible se transforma en un
    ///   <see cref="DbParam"/> con nombre <c>@{propiedad}</c>
    /// - El nombre del parámetro se genera en minúsculas
    /// - Las propiedades marcadas con <see cref="NotMappedAttribute"/>
    ///   o <see cref="NoSaveDbAttribute"/> son excluidas
    /// 
    /// Este método es utilizado por operaciones genéricas de
    /// inserción y actualización para evitar código repetitivo
    /// y mantener consistencia en el acceso a datos.
    /// </remarks>
    /// <returns>
    /// Lista de parámetros SQL representando el estado actual
    /// de la entidad.
    /// </returns>
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

    /// <summary>
    /// Ejecuta una consulta de conteo (<c>COUNT</c>) sobre una tabla
    /// o vista utilizando una condición personalizada.
    /// </summary>
    /// <param name="where">
    /// Cláusula WHERE de la consulta, sin incluir la palabra clave
    /// <c>WHERE</c>.
    /// </param>
    /// <param name="tableName">
    /// Nombre de la tabla o vista sobre la cual se ejecutará el conteo.
    /// Si es <c>null</c>, se utiliza la tabla asociada al repositorio.
    /// </param>
    /// <param name="parameters">
    /// Colección opcional de parámetros asociados a la cláusula WHERE.
    /// </param>
    /// <remarks>
    /// Este método es utilizado internamente por componentes como
    /// <see cref="QueryBuilder{T}"/> para obtener conteos consistentes
    /// utilizando el mismo mecanismo de parametrización y ejecución
    /// del repositorio.
    /// 
    /// No aplica paginación ni ordenamiento, ya que su único propósito
    /// es retornar la cantidad total de registros que cumplen la
    /// condición especificada.
    /// </remarks>
    /// <returns>
    /// Número total de registros que cumplen la condición.
    /// </returns>
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

    /// <summary>
    /// Determina si existe al menos un registro que cumpla con las
    /// condiciones especificadas.
    /// </summary>
    /// <param name="build">
    /// Acción que define dinámicamente las condiciones del
    /// <see cref="QueryBuilder{T}"/>.
    /// </param>
    /// <param name="excludeId">
    /// Identificador opcional de un registro a excluir del conteo,
    /// comúnmente utilizado en validaciones durante actualizaciones.
    /// </param>
    /// <remarks>
    /// Este método es utilizado para validar existencia de registros
    /// sin necesidad de traer datos completos.
    /// 
    /// El criterio de búsqueda se define externamente mediante
    /// el <paramref name="build"/>, permitiendo reutilización
    /// y composición flexible de reglas.
    /// </remarks>
    /// <returns>
    /// <c>true</c> si existe al menos un registro que cumpla las
    /// condiciones; de lo contrario, <c>false</c>.
    /// </returns>
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

    /// <summary>
    /// Determina si existe un registro que coincida con un conjunto
    /// de columnas y valores.
    /// </summary>
    /// <param name="columns">
    /// Colección de pares columna/valor que deben cumplirse
    /// simultáneamente.
    /// </param>
    /// <param name="excludeId">
    /// Identificador opcional de un registro a excluir del conteo.
    /// </param>
    /// <remarks>
    /// Este método es un atajo para validaciones comunes de unicidad
    /// (por ejemplo, combinaciones únicas de columnas).
    /// 
    /// Internamente delega en <see cref="Exists"/> para construir
    /// dinámicamente la consulta.
    /// </remarks>
    /// <returns>
    /// <c>true</c> si existe un registro que cumpla con todas las
    /// condiciones; de lo contrario, <c>false</c>.
    /// </returns>
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