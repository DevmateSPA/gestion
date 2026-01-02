using Gestion.presentation.views.util.buildersUi;
using Gestion.core.model;
using Gestion.core.model.detalles;
using System.Windows.Data;

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

        DataGridColumnRegistry.Register<DetalleOrdenTrabajo>(
            new() { Header = "Tipo Papel", Binding = "TipoPapel",  Width = 90 },
            new() { Header = "Cantidad", Binding = "Cantidad", Width = 100 },
            new() { Header = "Sobras", Binding = "Sobras", Width = 160 },
            new() { Header = "Sobr.", Binding = "Sobr,", Width = 160 },
            new() { Header = "Total", Binding = "Total", Width = 160 },
            new() { Header = "Tamaño Pliegos", Binding = "TamPliegos", Width = 160 },
            new() { Header = "Tamaño", Binding = "Tamanio", Width = 160 }
        );
    }
}