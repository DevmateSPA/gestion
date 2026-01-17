using System.Diagnostics;

namespace Gestion.presentation.views.util.buildersUi.data;

public delegate Task<IEnumerable<string>> SearchProviderDelegate(string query);

internal sealed class SearchCacheEntry
{
    public string BaseQuery { get; set; } = string.Empty;
    public List<string> Results { get; set; } = [];
}

public static class SearchDataProvider
{
    // Providers registrados (por key)
    private static readonly Dictionary<string, SearchProviderDelegate> _sources = [];

    // Cache por key (empresa ya está cerrada en el delegate)
    private static readonly Dictionary<string, SearchCacheEntry> _cache = [];

    public static void Register(string key, SearchProviderDelegate provider)
    {
        _sources[key] = provider;
    }

    public static async Task<IEnumerable<string>> Search(
        string key,
        string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        if (!_sources.TryGetValue(key, out var provider))
            return [];

    if (_cache.TryGetValue(key, out var cache))
    {
        if (query.Length > cache.BaseQuery.Length &&
            query.StartsWith(cache.BaseQuery, StringComparison.OrdinalIgnoreCase))
        {
            return cache.Results;
        }

        // borrar cache si el usuario retrocede
        if (query.Length < cache.BaseQuery.Length)
        {
            _cache.Remove(key);
        }
    }

        var results = (await provider(query)).ToList();

        // No cachear resultados vacíos
        if (results.Count > 0)
        {
            _cache[key] = new SearchCacheEntry
            {
                BaseQuery = query,
                Results = results
            };
        }

        return results;
    }
}