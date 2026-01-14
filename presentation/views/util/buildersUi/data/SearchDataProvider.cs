namespace Gestion.presentation.views.util.buildersUi.data;

public delegate Task<IEnumerable<string>> SearchProviderDelegate(string query);

public static class SearchDataProvider
{
    private static readonly Dictionary<string, SearchProviderDelegate> _sources = [];

    public static void Register(string key, SearchProviderDelegate provider)
    {
        _sources[key] = provider;
    }

    public static Task<IEnumerable<string>> Search(string key, string query)
    {
        if (_sources.TryGetValue(key, out var provider))
            return provider(query);

        return Task.FromResult(Enumerable.Empty<string>());
    }
}