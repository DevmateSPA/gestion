using System.Threading.Tasks;
using System.Windows;
using Gestion.presentation.views.windows;

namespace Gestion.helpers;

public class EditorEntidadBuilder<T>
    where T : class
{
    private Window? _owner;
    private T? _entidad;
    private string _titulo = "Editar";
    private Func<T, Task<bool>>? _guardar;
    private Func<T, Task>? _onClose;

    public EditorEntidadBuilder<T> Owner(Window owner)
    {
        _owner = owner;
        return this;
    }

    public EditorEntidadBuilder<T> Entidad(T entidad)
    {
        _entidad = entidad;
        return this;
    }

    public EditorEntidadBuilder<T> Titulo(string titulo)
    {
        _titulo = titulo;
        return this;
    }

    public EditorEntidadBuilder<T> Guardar(Func<T, Task<bool>> guardar)
    {
        _guardar = guardar;
        return this;
    }

    public EditorEntidadBuilder<T> OnClose(Func<T, Task> onClose)
    {
        _onClose = onClose;
        return this;
    }

    public async Task Abrir()
    {
        if (_entidad == null)
            throw new InvalidOperationException("Entidad no definida");
        if (_guardar == null)
            throw new InvalidOperationException("Acción Guardar no definida");

        var ventana = new EntidadEditorTableWindow(
            _entidad,
            async obj => await _guardar!((T)obj),
            _titulo)
        {
            Owner = _owner
        };

        var result = ventana.ShowDialog();

        if (result == true && _onClose != null)
            await _onClose((T)ventana.EntidadEditada!);
    }
}
