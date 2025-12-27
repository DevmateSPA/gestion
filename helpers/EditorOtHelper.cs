using System.Windows;
using Gestion.core.model;
using Gestion.core.model.detalles;
using Gestion.presentation.views.pages;

namespace Gestion.helpers;

public static class EditorOtHelper
{
    public static bool Abrir(
        Window owner,
        OrdenTrabajo entidad,
        Func<OrdenTrabajo, Task<bool>> accion,
        Func<OrdenTrabajo, Task> syncDetalles)
    {
        var ventana = new OrdenTrabajoDetallePage(entidad, accion, syncDetalles)
        {
            Owner = owner
        };

        return ventana.ShowDialog() == true;
    }
}