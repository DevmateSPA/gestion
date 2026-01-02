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
            new() { Header = "Código", Binding = "Producto", Width = 100 },
            new() { Header = "Nombre", Binding = "Productonombre", Width = 800},
            new() { Header = "Cantidad", Binding = "Entrada", Width = 100}
        );
    }
}