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

public partial class EntidadEditorTableWindow: Window
{
    // Builder de formularios
    private readonly FormularioBuilder _formularioBuilder = new();
    private readonly DataGridBuilder<FacturaCompraProducto> _dataGridBuilder = new();
    private readonly FacturaCompra _entidadOriginal;
    private readonly Func<FacturaCompra, Task<bool>> _accion;
    private readonly Func<FacturaCompra, Task>? _syncDetalles;
    private Dictionary<PropertyInfo, FrameworkElement> _controles = [];

    private DataGrid? dgDetalles;

    public FacturaCompra? EntidadEditada { get; private set; }

    public EntidadEditorTableWindow(
        FacturaCompra entidad,
        Func<FacturaCompra, Task<bool>> accion,
        Func<FacturaCompra, Task>? syncDetalles,
        string titulo = "Ventana con tabla")
    {
        InitializeComponent();
        Title = titulo;

        _entidadOriginal = entidad;
        _accion = accion;
        _syncDetalles = syncDetalles;

        ClonarEntidad(entidad);

        InicializarUI(EntidadEditada!);
        InicializarEventos();
    }

    private void ClonarEntidad(FacturaCompra entidad)
    {
        EntidadEditada = (FacturaCompra)Activator.CreateInstance(entidad.GetType())!;

        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }
    }

    private void InicializarUI(FacturaCompra entidad)
    {
        // Crear el builder para la entidad
        var builder = new VentanaBuilder<FacturaCompra>()
            .SetEntidad(entidad)
            .SetContenedorCampos(spCampos)
            .SetContenedorTablas(spTabla)
            .SetModo(ModoFormulario.SoloLectura);

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