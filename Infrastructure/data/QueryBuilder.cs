using System.Reflection;
using Gestion.core.interfaces.model;

namespace Gestion.Infrastructure.data;

/// <summary>
/// Builder fluido para la construcción de consultas SELECT dinámicas
/// sobre una entidad del dominio.
///
/// Permite definir de forma incremental:
/// - Tabla o vista de origen
/// - Columnas seleccionadas
/// - Condiciones WHERE parametrizadas
/// - Ordenamiento
/// - Paginación (LIMIT / OFFSET)
///
/// El QueryBuilder NO ejecuta SQL directamente, sino que delega la
/// ejecución al repositorio base, manteniendo la responsabilidad de
/// acceso a datos en la capa de infraestructura.
/// </summary>
/// <typeparam name="T">
/// Entidad del dominio asociada al repositorio.
/// Debe implementar IModel y tener constructor sin parámetros.
/// </typeparam>
/// <remarks>
/// Uso interno:
/// - No debe recibir input directo del usuario para nombres de columnas,
///   tablas o cláusulas SQL.
/// - Está pensado para ser utilizado exclusivamente por repositorios
///   o servicios de infraestructura.
///
/// Objetivo:
/// - Reducir duplicación de SQL
/// - Mantener consultas legibles y encadenables
/// - Evitar errores de parámetros duplicados
/// </remarks>
public class QueryBuilder<T> where T : IModel, new()
{
    private readonly BaseRepository<T> _repo;
    private readonly List<string> _whereClauses = [];
    private readonly List<DbParam> _parameters = [];
    private string? _orderBy = null;
    private int? _limit = null;
    private int? _offset = null;
    private string? _from = null;
    private string _selectColumns = "*";

    public QueryBuilder(BaseRepository<T> repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Define explícitamente la tabla o vista desde la cual se realizará la consulta.
    /// Si no se especifica, se utiliza la vista o tabla configurada en el repositorio.
    /// </summary>
    public QueryBuilder<T> From(string tableOrView)
    {
        _from = tableOrView;
        return this;
    }

    /// <summary>
    /// Define las columnas a seleccionar en la consulta.
    /// Si no se especifica, se seleccionan todas las columnas (*).
    ///
    /// Cuando se utiliza junto a ToListAsync&lt;TData&gt;,
    /// debe coincidir con el nombre de una propiedad de la entidad.
    /// </summary>
    public QueryBuilder<T> Select(params string[] columns)
    {
        if (columns != null && columns.Length > 0)
            _selectColumns = string.Join(", ", columns);
        return this;
    }

    /// <summary>
    /// Agrega una condición WHERE arbitraria a la consulta.
    /// Permite múltiples llamadas, las cuales se combinan usando AND.
    ///
    /// La condición debe utilizar parámetros SQL.
    /// </summary>
    public QueryBuilder<T> Where(string condition, params DbParam[] parameters)
    {
        if (!string.IsNullOrWhiteSpace(condition))
            _whereClauses.Add(condition);

        if (parameters != null && parameters.Length > 0)
            _parameters.AddRange(parameters);

        return this;
    }

    /// <summary>
    /// Agrega una condición de igualdad parametrizada.
    /// El nombre del parámetro se genera automáticamente para evitar colisiones.
    /// </summary>
    public QueryBuilder<T> WhereEqual(string column, object value)
    {
        string paramName = $"@p{_parameters.Count}";
        _whereClauses.Add($"{column} = {paramName}");
        _parameters.Add(new DbParam(paramName, value));
        return this;
    }

    /// <summary>
    /// Agrega una condición BETWEEN parametrizada.
    /// </summary>
    public QueryBuilder<T> WhereBetween(string column, object from, object to)
    {
        string pFrom = $"@p{_parameters.Count}";
        string pTo   = $"@p{_parameters.Count + 1}";

        _whereClauses.Add($"{column} BETWEEN {pFrom} AND {pTo}");
        _parameters.Add(new DbParam(pFrom, from));
        _parameters.Add(new DbParam(pTo, to));

        return this;
    }

    /// <summary>
    /// Agrega una condición IS NULL a la consulta.
    /// </summary>
    public QueryBuilder<T> WhereIsNull(string column)
    {
        _whereClauses.Add($"{column} IS NULL");
        return this;
    }

    /// <summary>
    /// Agrega una condición LIKE parametrizada.
    /// </summary>
    public QueryBuilder<T> WhereLike(string column, string value)
    {
        string paramName = $"@p{_parameters.Count}";
        _whereClauses.Add($"{column} LIKE {paramName}");
        _parameters.Add(new DbParam(paramName, value));
        return this;
    }

    /// <summary>
    /// Define la cláusula ORDER BY de la consulta.
    /// </summary>
    public QueryBuilder<T> OrderBy(string orderBy)
    {
        _orderBy = orderBy;
        return this;
    }

    /// <summary>
    /// Aplica paginación basada en número de página y tamaño de página.
    /// </summary>
    public QueryBuilder<T> Page(int pageNumber, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageNumber);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        _limit = pageSize;
        _offset = (pageNumber - 1) * pageSize;

        return this;
    }

    /// <summary>
    /// Define directamente el LIMIT y opcionalmente el OFFSET de la consulta.
    /// </summary>
    public QueryBuilder<T> Limit(int limit, int? offset = null)
    {
        _limit = limit;
        if (offset != null)
            _offset = offset;
        return this;
    }

    /// <summary>
    /// Construye la cláusula WHERE final combinando todas las condiciones
    /// mediante AND.  
    /// Si no existen condiciones, retorna una expresión siempre verdadera (1=1).
    /// </summary>
    private string BuildWhereClause()
    {
        if (_whereClauses.Count == 0)
            return "1=1";

        return string.Join(" AND ", _whereClauses);
    }

    /// <summary>
    /// Ejecuta la consulta construida y retorna una lista de resultados.
    ///
    /// Permite:
    /// - Retornar entidades completas (T)
    /// - Retornar un tipo escalar o DTO simple (TData) cuando se selecciona
    ///   una sola columna
    /// </summary>
    /// <typeparam name="TData">
    /// Tipo de datos esperado en el resultado.
    /// </typeparam>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si TData es distinto de T y no se ha definido una columna válida
    /// en Select().
    /// </exception>
    public async Task<List<TData>> ToListAsync<TData>()
    {
        string select = _selectColumns ?? "*";

        var result = await _repo.FindWhereFrom(
            tableOrView: _from ?? _repo._viewName ?? _repo._tableName,
            where: BuildWhereClause(),
            orderBy: _orderBy,
            limit: _limit,
            offset: _offset,
            parameters: _parameters,
            selectColumns: select
        );

        // Mapear solo la columna TData si es necesario
        if (typeof(TData) != typeof(T))
        {
            if (_selectColumns == "*" || string.IsNullOrEmpty(_selectColumns))
                throw new InvalidOperationException("_selectColumns debe ser el nombre de una propiedad si TData != T");

            var propInfo = typeof(T).GetProperty(
                _selectColumns!,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase
            );

            if (propInfo == null)
                throw new InvalidOperationException($"La propiedad '{_selectColumns}' no existe en {typeof(T).Name}");

            return [.. result.Select(r =>
            {
                var value = propInfo.GetValue(r);  // puede ser null
                return (TData?)_repo.ConvertValue(value, typeof(TData))!;
            })];
        }

        return [.. result.Cast<TData>()];
    }

    /// <summary>
    /// Ejecuta una consulta COUNT(*) utilizando las mismas condiciones WHERE
    /// construidas en el builder.
    /// </summary>
    public async Task<long> CountAsync()
    {
        var table = _from ?? _repo._tableName;
        return await _repo.CountWhere(
            where: BuildWhereClause(),
            tableName: table,
            parameters: _parameters
        );
    }
}