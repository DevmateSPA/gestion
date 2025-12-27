using System.Windows;
using Gestion.core.interfaces.model;
using Gestion.core.model;
using Gestion.presentation.views.windows;

namespace Gestion.helpers;

public static class EditorTableHelper
{
    public static bool Abrir(
        Window owner,
        FacturaCompra entidadConDetalles,
        IEnumerable<FacturaCompraProducto> detalles,
        Func<FacturaCompra, Task<bool>> accion,
        Func<FacturaCompra, Task>? syncDetalles,
        string titulo)
    {
        var ventana = new EntidadEditorTableWindow(
            entidadConDetalles,
            detalles,
            accion,
            syncDetalles,
            titulo)
        {
            Owner = owner
        };

        return ventana.ShowDialog() == true;
    }
}