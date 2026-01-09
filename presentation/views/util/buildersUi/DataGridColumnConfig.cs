namespace Gestion.presentation.views.util.buildersUi;

/// <summary>
/// Define la configuración de una columna para un DataGrid.
/// 
/// Esta clase es utilizada por <see cref="DataGridColumnRegistry"/>
/// para registrar columnas asociadas a un tipo de entidad,
/// permitiendo construir DataGrids de forma declarativa y reutilizable.
/// </summary>
public class DataGridColumnConfig
{
    /// <summary>
    /// Texto que se mostrará en el encabezado de la columna.
    /// </summary>
    public string Header { get; init; } = "";

    /// <summary>
    /// Ruta de binding hacia la propiedad de la entidad.
    /// 
    /// Ejemplo: "Nombre", "Cliente.RazonSocial".
    /// </summary>
    public string Binding { get; init; } = "";

    /// <summary>
    /// Ancho fijo de la columna en píxeles.
    /// 
    /// Si es <c>null</c>, el ancho será calculado automáticamente
    /// por el DataGrid o podrá ajustarse a espacio disponible.
    /// </summary>
    public double? Width { get; init; }
}
