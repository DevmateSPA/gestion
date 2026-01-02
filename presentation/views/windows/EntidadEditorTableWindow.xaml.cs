using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.interfaces.model;
using Gestion.core.model;
using Gestion.helpers;
using Gestion.presentation.enums;
using Gestion.presentation.views.util;
using Gestion.presentation.views.util.buildersUi;

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorTableWindow<TEntidad, TDetalle> : Window
    where TEntidad : class
{
    // Builder de formularios
    private readonly FormularioBuilder _formularioBuilder = new();
    private readonly DataGridBuilder<TDetalle> _dataGridBuilder = new();
    private readonly TEntidad _entidadOriginal;
    private readonly IEnumerable<TDetalle>? _detalles;
    private readonly Func<TEntidad, Task<bool>> _accion;
    private readonly Func<TEntidad, Task>? _syncDetalles;
    private Dictionary<PropertyInfo, FrameworkElement> _controles = [];

    private DataGrid? dgDetalles;

    public TEntidad? EntidadEditada { get; private set; }

    public EntidadEditorTableWindow(
        TEntidad entidad,
        IEnumerable<TDetalle> detalles,
        Func<TEntidad, Task<bool>> accion,
        Func<TEntidad, Task>? syncDetalles,
        string titulo = "Ventana con tabla")
    {
        InitializeComponent();
        Title = titulo;

        _entidadOriginal = entidad;
        _accion = accion;
        _syncDetalles = syncDetalles;
        _detalles = detalles;

        ClonarEntidad(entidad);

        InicializarUI(EntidadEditada!);
        InicializarEventos();
    }

    private void ClonarEntidad(TEntidad entidad)
    {
        EntidadEditada = (TEntidad)Activator.CreateInstance(entidad.GetType())!;

        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }
    }

    private void InicializarUI(TEntidad entidad)
    {
        // Crear el builder para la entidad
        var builder = new VentanaBuilder<TEntidad>()
            .SetEntidad(entidad)
            .SetContenedorCampos(spCampos)
            .SetContenedorTablas(spTabla)
            .SetModo(ModoFormulario.Edicion);

        // Se genera la UI
        builder.Build();

        // Recuperamos los controles
        _controles = builder.GetControles();

        // Validamos los campos inicialmente
        FormularioValidator.ForzarValidacionInicial(_controles);

        // Se enfooca el control
        _controles.Values.FirstOrDefault()?.Focus();
    }

    private void EnfocarPrimerControl()
    {
        _controles.Values.FirstOrDefault()?.Focus();
    }

    private void InicializarEventos()
    {
        PreviewKeyDown += (_, e) =>
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        };
    }

    // -----------------------------
    //   DATA GRID DE DETALLES
    // -----------------------------

    private bool Validar()
    {
        var errores = ValidationHelper.GetValidationErrors(spCampos);

        if (errores.Count == 0)
            return true;

        DialogUtils.MostrarErroresValidacion(errores);
        return false;
    }

    private async Task EjecutarAcción()
    {
        if (_accion != null)
        {
            if (EntidadEditada == null)
                throw new InvalidOperationException("Entidad editada no definida.");

            bool ok = await _accion(EntidadEditada);

            if (!ok)
                return;

            if (_syncDetalles != null)
                await _syncDetalles(EntidadEditada);
        }

        DialogUtils.MostrarInfo(Mensajes.OperacionExitosa, "Éxito");
        DialogResult = true;
        Close();
    }

    private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        if (!Validar())
            return;

        await EjecutarAcción();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}