namespace Gestion.presentation.views.util.buildersUi;

/// <summary>
/// Registro centralizado de configuraciones de columnas para DataGrid,
/// asociadas a un tipo de entidad.
/// 
/// Permite definir una sola vez qué columnas debe mostrar un DataGrid
/// para una entidad específica y reutilizar esa configuración en
/// distintos builders o vistas.
/// </summary>
public static class DataGridColumnRegistry
{
    /// <summary>
    /// Diccionario interno que asocia un tipo de entidad
    /// con su lista de configuraciones de columnas.
    /// </summary>
    private static readonly Dictionary<Type, List<DataGridColumnConfig>> _configs
        = [];

    /// <summary>
    /// Registra la configuración de columnas para un tipo de entidad.
    /// 
    /// Si el tipo ya tenía columnas registradas, estas se sobrescriben.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de la entidad que será representada en el DataGrid.
    /// </typeparam>
    /// <param name="columns">
    /// Conjunto de configuraciones de columnas asociadas al tipo.
    /// </param>
    public static void Register<T>(params DataGridColumnConfig[] columns)
    {
        _configs[typeof(T)] = [.. columns];
    }

    /// <summary>
    /// Obtiene la configuración de columnas registrada para un tipo de entidad.
    /// </summary>
    /// <param name="type">
    /// Tipo de la entidad cuyo DataGrid se desea construir.
    /// </param>
    /// <returns>
    /// Lista de configuraciones de columnas si existen para el tipo;
    /// en caso contrario, <c>null</c>.
    /// </returns>
    public static List<DataGridColumnConfig>? Get(Type type)
        => _configs.TryGetValue(type, out var cols) ? cols : null;
}
