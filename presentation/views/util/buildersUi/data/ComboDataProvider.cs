namespace Gestion.presentation.views.util.buildersUi.data;

public static class ComboDataProvider
{
    private static readonly Dictionary<string, IEnumerable<object>> _sources = [];

    public static void Register(string key, IEnumerable<object> data)
        => _sources[key] = data;

    public static IEnumerable<object> Get(string key)
        => _sources.TryGetValue(key, out var data) ? data : [];
}