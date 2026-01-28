using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Gestion.presentation.enums;

namespace Gestion.presentation.views.util.buildersUi;

/// <summary>
/// Builder principal encargado de componer una ventana de edición/visualización
/// de una entidad, integrando formulario de campos, tablas de detalles y botones
/// de acción.
/// </summary>
/// <typeparam name="TEntidad">
/// Tipo de la entidad que será visualizada o editada en la ventana.
/// </typeparam>
/// <remarks>
/// <para>
/// Este builder actúa como orquestador de:
/// <list type="bullet">
/// <item><see cref="FormularioBuilder"/> para los campos simples.</item>
/// <item><see cref="DetallesBuilder"/> para colecciones asociadas.</item>
/// <item>La visibilidad y estado de botones según el <see cref="ModoFormulario"/>.</item>
/// </list>
/// </para>
/// <para>
/// No crea controles directamente, sino que coordina builders especializados,
/// manteniendo una separación clara de responsabilidades.
/// </para>
/// </remarks>
public class VentanaBuilder<TEntidad>
{
    private readonly FormularioBuilder _fomularioBuilder = new();
    private readonly DetallesBuilder _detallesBuilder = new();

    private Dictionary<PropertyInfo, FrameworkElement> _controles = [];
    private Panel? _contenedorCampos;
    private Panel? _contenedorTablas;
    private Button? _btnGuardar;
    private Button? _btnImprimir;
    private Button? _btnEntregar;
    private bool? _shouldImprimir;
    private TEntidad? _entidad;
    private ModoFormulario _modo = ModoFormulario.Edicion;

    /// <summary>
    /// Define la entidad que será utilizada como fuente de datos
    /// para la ventana.
    /// </summary>
    public VentanaBuilder<TEntidad> SetEntidad(TEntidad entidad)
    {
        _entidad = entidad;
        return this;
    }

    /// <summary>
    /// Define el contenedor visual donde se renderizarán
    /// los campos del formulario.
    /// </summary>
    public VentanaBuilder<TEntidad> SetContenedorCampos(Panel contenedor)
    {
        _contenedorCampos = contenedor;
        return this;
    }

    /// <summary>
    /// Define el contenedor visual donde se renderizarán
    /// las tablas de detalles (colecciones).
    /// </summary>
    public VentanaBuilder<TEntidad> SetContenedorTablas(Panel contenedor)
    {
        _contenedorTablas = contenedor;
        return this;
    }

    /// <summary>
    /// Asocia el botón de guardado, cuya visibilidad
    /// dependerá del modo del formulario.
    /// </summary>
    public VentanaBuilder<TEntidad> SetBotonGuardar(Button btn)
    {
        _btnGuardar = btn;
        return this;
    }

    /// <summary>
    /// Asocia el botón de impresión y define si debe mostrarse
    /// en la ventana.
    /// </summary>
    /// <param name="btn">Botón de impresión.</param>
    /// <param name="shouldImprimir">
    /// Indica si la funcionalidad de impresión está habilitada
    /// para esta ventana.
    /// </param>
    public VentanaBuilder<TEntidad> SetBotonImprimir(Button btn, bool shouldImprimir)
    {
        _btnImprimir = btn;
        _shouldImprimir = shouldImprimir;
        return this;
    }

    public VentanaBuilder<TEntidad> SetBotonEntregar(Button btn, string label)
    {
        _btnEntregar = btn;
        _btnEntregar.Content = label;
        return this;
    }

    /// <summary>
    /// Define el modo en el que se construirá la ventana
    /// (edición o solo lectura).
    /// </summary>
    public VentanaBuilder<TEntidad> SetModo(ModoFormulario modo)
    {
        _modo = modo;
        return this;
    }

    /// <summary>
    /// Construye completamente la ventana, creando el formulario,
    /// los detalles y configurando la visibilidad de los botones.
    /// </summary>
    /// <remarks>
    /// Este método debe invocarse una vez que todos los componentes
    /// necesarios han sido configurados mediante los métodos <c>Set*</c>.
    /// </remarks>
    public void Build()
    {
        AplicarVisibilidadBotones();

        _fomularioBuilder
            .SetEntidad(_entidad!)
            .SetContenedor(_contenedorCampos!)
            .SetControles(_controles)
            .SetModo(_modo)
            .SetMaxFila(3)
            .Build();

        _detallesBuilder
            .SetEntidad(_entidad!)
            .SetContenedor(_contenedorTablas!)
            .SetModo(_modo)
            .Build();
    }

    /// <summary>
    /// Aplica la visibilidad y el texto de los botones de acción
    /// según el modo del formulario y la configuración de impresión.
    /// </summary>
    private void AplicarVisibilidadBotones()
    {
        if (_btnGuardar != null)
        {
            _btnGuardar.Visibility =
                _modo == ModoFormulario.Edicion
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        if (_btnImprimir != null)
        {
            _btnImprimir.Visibility =
                _shouldImprimir == true
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            // Cambia el texto a "Guardar e Imprimir" si la ventana
            // está en modo edición y la impresión está habilitada.
            bool guardarEImprimir =
                _shouldImprimir == true && _modo == ModoFormulario.Edicion;

            _btnImprimir.Content = guardarEImprimir
                ? "Guardar e Imprimir"
                : "Imprimir";
        }

        if (_btnEntregar != null)
        {
            _btnEntregar.Visibility = Visibility.Visible;
        }
    }

    /// <summary>
    /// Obtiene el diccionario que asocia cada propiedad de la entidad
    /// con su control visual correspondiente.
    /// </summary>
    /// <returns>
    /// Diccionario de controles generados por el formulario.
    /// </returns>
    public Dictionary<PropertyInfo, FrameworkElement> GetControles() => _controles;
}