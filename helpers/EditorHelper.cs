using System.Windows;
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
}