namespace Gestion.presentation.views.util.buildersUi;

public static class DataGridColumnRegistry
{
    private static readonly Dictionary<Type, List<DataGridColumnConfig>> _configs
        = [];

    public static void Register<T>(params DataGridColumnConfig[] columns)
    {
        _configs[typeof(T)] = [.. columns];
    }

    public static List<DataGridColumnConfig>? Get(Type type)
        => _configs.TryGetValue(type, out var cols) ? cols : null;
}