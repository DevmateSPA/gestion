using System.Reflection;
using Gestion.core.interfaces.model;
using Gestion.Infrastructure.data;

namespace Gestion.Infrastructure.data;
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

    public QueryBuilder<T> From(string tableOrView)
    {
        _from = tableOrView;
        return this;
    }

    public QueryBuilder<T> Select(params string[] columns)
    {
        if (columns != null && columns.Length > 0)
            _selectColumns = string.Join(", ", columns);
        return this;
    }

    // Permite múltiples llamadas a Where y acumula las condiciones
    public QueryBuilder<T> Where(string condition, params DbParam[] parameters)
    {
        if (!string.IsNullOrWhiteSpace(condition))
            _whereClauses.Add(condition);

        if (parameters != null && parameters.Length > 0)
            _parameters.AddRange(parameters);

        return this;
    }

    public QueryBuilder<T> WhereEqual(string column, object value)
    {
        string paramName = $"@p{_parameters.Count}";
        _whereClauses.Add($"{column} = {paramName}");
        _parameters.Add(new DbParam(paramName, value));
        return this;
    }

    public QueryBuilder<T> WhereBetween(string column, object from, object to)
    {
        string pFrom = $"@p{_parameters.Count}";
        string pTo   = $"@p{_parameters.Count + 1}";

        _whereClauses.Add($"{column} BETWEEN {pFrom} AND {pTo}");
        _parameters.Add(new DbParam(pFrom, from));
        _parameters.Add(new DbParam(pTo, to));

        return this;
    }

    public QueryBuilder<T> WhereIsNull(string column)
    {
        _whereClauses.Add($"{column} IS NULL");
        return this;
    }

    public QueryBuilder<T> WhereLike(string column, string value)
    {
        string paramName = $"@p{_parameters.Count}";
        _whereClauses.Add($"{column} LIKE {paramName}");
        _parameters.Add(new DbParam(paramName, value));
        return this;
    }

    public QueryBuilder<T> OrderBy(string orderBy)
    {
        _orderBy = orderBy;
        return this;
    }

    public QueryBuilder<T> Page(int pageNumber, int pageSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageNumber);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        _limit = pageSize;
        _offset = (pageNumber - 1) * pageSize;

        return this;
}

    public QueryBuilder<T> Limit(int limit, int? offset = null)
    {
        _limit = limit;
        if (offset != null)
            _offset = offset;
        return this;
    }

    // Combina todas las condiciones con AND
    private string BuildWhereClause()
    {
        if (_whereClauses.Count == 0)
            return "1=1";

        return string.Join(" AND ", _whereClauses);
    }

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