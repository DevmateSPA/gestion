using System.Windows;
using Gestion.core.interfaces.model;
using Gestion.presentation.views.util;
using Gestion.presentation.views.windows;

namespace Gestion.helpers;

public static class EditorHelper
{
    public static bool Abrir(
        Window owner,
        object entidad,
        Func<object, Task<bool>> accion,
        string titulo)
    {
        var ventana = new EntidadEditorWindow(entidad, accion, titulo)
        {
            Owner = owner
        };

        return ventana.ShowDialog() == true;
    }

    public static async Task BorrarSeleccionado<T>(
        T? seleccionado,
        Func<T, Task> borrarAccion,
        string mensajeConfirmacion,
        string mensajeExito,
        string mensajeSeleccionVacia = "Selecciona un registro antes de eliminar.")
        where T : IEmpresa
    {
        if (seleccionado == null)
        {
            DialogUtils.MostrarAdvertencia(mensajeSeleccionVacia, "Aviso");
            return;
        }

        if (!DialogUtils.Confirmar(mensajeConfirmacion, "Confirmar eliminación"))
            return;

        await borrarAccion(seleccionado);

        DialogUtils.MostrarInfo(mensajeExito, "Éxito");
    }
}