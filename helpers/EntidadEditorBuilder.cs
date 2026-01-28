using System.Threading.Tasks;
using System.Windows;
using Gestion.core.model;
using Gestion.presentation.enums;
using Gestion.presentation.views.windows;

namespace Gestion.helpers;

/// <summary>
/// Builder fluido para configurar y abrir una ventana de edición
/// de una entidad de dominio.
/// </summary>
/// <typeparam name="T">
/// Tipo de la entidad que será editada.
/// </typeparam>
/// <remarks>
/// Este builder permite configurar:
/// <list type="bullet">
/// <item>La ventana propietaria.</item>
/// <item>La entidad a editar.</item>
/// <item>El título de la ventana.</item>
/// <item>La lógica de guardado.</item>
/// <item>La impresión asociada (opcional).</item>
/// <item>El modo del formulario (edición o solo lectura).</item>
/// <item>Acciones a ejecutar al cerrar la ventana.</item>
/// </list>
/// El patrón builder facilita una configuración clara y legible
/// mediante encadenamiento de métodos.
/// </remarks>
public class EditorEntidadBuilder<T>
    where T : class
{
    private Window? _owner;
    private T? _entidad;
    private string _titulo = "Editar";
    private Func<T, Task<bool>>? _guardar = _ => Task.FromResult(true);
    private Action<OrdenTrabajo>? _imprimir = null;
    private Func<EntidadEditorTableWindow, Task>? _btn1Action = null;
    private Func<T, Task>? _onClose;
    private ModoFormulario _modo = ModoFormulario.Edicion;
    private bool _ShouldImpresion = false;
	private string _tituloBtnEntregar = "Entregar (F5)";


    /// <summary>
    /// Define la ventana propietaria del editor.
    /// </summary>
    /// <param name="owner">Ventana que será la dueña del editor.</param>
    /// <returns>La instancia del builder para encadenamiento.</returns>
    public EditorEntidadBuilder<T> Owner(Window owner)
    {
        _owner = owner;
        return this;
    }

    /// <summary>
    /// Define la entidad que será editada.
    /// </summary>
    /// <param name="entidad">Entidad a editar.</param>
    /// <returns>La instancia del builder para encadenamiento.</returns>
    public EditorEntidadBuilder<T> Entidad(T entidad)
    {
        _entidad = entidad;
        return this;
    }

    /// <summary>
    /// Define el título de la ventana de edición.
    /// </summary>
    /// <param name="titulo">Texto del título.</param>
    /// <returns>La instancia del builder para encadenamiento.</returns>
    public EditorEntidadBuilder<T> Titulo(string titulo)
    {
        _titulo = titulo;
        return this;
    }

    public EditorEntidadBuilder<T> TituloBtnExtra1(string titulo)
    {
        _tituloBtnEntregar = titulo;
        return this;
    }

    public EditorEntidadBuilder<T> SetBtn1Action(Func<EntidadEditorTableWindow, Task> action)
    {
        _btn1Action = action;
        return this;
    }

    /// <summary>
    /// Define la lógica de guardado de la entidad.
    /// </summary>
    /// <param name="guardar">
    /// Función que recibe la entidad editada y retorna
    /// <c>true</c> si el guardado fue exitoso.
    /// </param>
    /// <returns>La instancia del builder para encadenamiento.</returns>
    public EditorEntidadBuilder<T> Guardar(Func<T, Task<bool>> guardar)
    {
        _guardar = guardar;
        return this;
    }

    /// <summary>
    /// Define la acción de impresión asociada a la entidad.
    /// </summary>
    /// <param name="imprimir">
    /// Acción que recibe una <see cref="OrdenTrabajo"/> para imprimir.
    /// </param>
    /// <returns>La instancia del builder para encadenamiento.</returns>
    public EditorEntidadBuilder<T> Imprimir(Action<OrdenTrabajo> imprimir)
    {
        _imprimir = imprimir;
        return this;
    }

    /// <summary>
    /// Indica que el botón de impresión debe mostrarse.
    /// </summary>
    /// <returns>La instancia del builder para encadenamiento.</returns>
    public EditorEntidadBuilder<T> ShouldBtnImpresion()
    {
        _ShouldImpresion = true;
        return this;
    }

    /// <summary>
    /// Define una acción asincrónica a ejecutar al cerrar la ventana,
    /// solo si la edición fue confirmada.
    /// </summary>
    /// <param name="onClose">
    /// Acción que recibe la entidad editada.
    /// </param>
    /// <returns>La instancia del builder para encadenamiento.</returns>
    public EditorEntidadBuilder<T> OnClose(Func<T, Task> onClose)
    {
        _onClose = onClose;
        return this;
    }

    /// <summary>
    /// Configura el formulario en modo solo lectura.
    /// </summary>
    /// <returns>La instancia del builder para encadenamiento.</returns>
    public EditorEntidadBuilder<T> SoloLecutra()
    {
        _modo = ModoFormulario.SoloLectura;
        return this;
    }

    /// <summary>
    /// Configura el formulario en modo edición.
    /// </summary>
    /// <returns>La instancia del builder para encadenamiento.</returns>
    public EditorEntidadBuilder<T> Edicion()
    {
        _modo = ModoFormulario.Edicion;
        return this;
    }

    /// <summary>
    /// Define explícitamente el modo del formulario.
    /// </summary>
    /// <param name="modo">Modo del formulario.</param>
    /// <returns>La instancia del builder para encadenamiento.</returns>
    public EditorEntidadBuilder<T> Modo(ModoFormulario modo)
    {
        _modo = modo;
        return this;
    }


    public EditorEntidadBuilder<T> TituloBtnEntregar(string titulo)
    {
        _tituloBtnEntregar = titulo;
        return this;
    }

    public EditorEntidadBuilder<T> Entregar(Func<EntidadEditorTableWindow, Task> action)
    {
        _btn1Action = action;
        return this;
    }

    /// <summary>
    /// Construye y abre la ventana de edición configurada.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si la entidad no ha sido definida.
    /// </exception>
    public async Task Abrir()
    {
        if (_entidad == null)
            throw new InvalidOperationException("Entidad no definida");

        var ventana = new EntidadEditorTableWindow(
            _entidad,
            async obj => _guardar == null || await _guardar((T)obj),
            _imprimir,
            _btn1Action,
            _modo,
            _ShouldImpresion,
            _titulo,
            _tituloBtnEntregar)

        {
            Owner = _owner
        };

        var result = ventana.ShowDialog();

        if (result == true && _onClose != null)
            await _onClose((T)ventana.EntidadEditada!);
    }
}