using Gestion.presentation.views.util.buildersUi;
using Gestion.core.model;

namespace Gestion.presentation.config;

public static class UiConfig
{
    public static void Register()
    {
        RegisterDataGrids();
    }

    private static void RegisterDataGrids()
    {
        // Dg de FacturaCompraProducto
        DataGridColumnRegistry.Register<FacturaCompraProducto>(
            new() { Header = "Código", Binding = "Producto" },
            new() { Header = "Nombre", Binding = "Productonombre"},
            new() { Header = "Cantidad", Binding = "Entrada"}
        );
    }
}