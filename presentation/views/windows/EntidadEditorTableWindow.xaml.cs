using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Gestion.core.model;
using Gestion.helpers;
using Gestion.presentation.views.util;
using Gestion.presentation.views.util.buildersUi;

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorTableWindow : Window
{
    // Builder de formularios
    private readonly FormularioBuilder _formularioBuilder = new();
    private readonly FacturaCompra _entidadOriginal;
    private readonly IEnumerable<FacturaCompraProducto>? _detalles;
    private readonly Func<FacturaCompra, Task<bool>> _accion;
    private readonly Func<FacturaCompra, Task> _syncDetalles;
    private readonly Dictionary<PropertyInfo, Control> _controles = [];

    public FacturaCompra EntidadEditada { get; private set; }

    public EntidadEditorTableWindow(
        FacturaCompra entidad,
        IEnumerable<FacturaCompraProducto> detalles,
        Func<FacturaCompra, Task<bool>> accion,
        Func<FacturaCompra, Task> syncDetalles,
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

    private void ClonarEntidad(FacturaCompra entidad)
    {
        EntidadEditada = (FacturaCompra)Activator.CreateInstance(entidad.GetType())!;

        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }
    }

    private void InicializarUI(object entidad)
    {
        GenerarCampos(entidad);
        CargarTabla();
        EnfocarPrimerControl();
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

    // --------------------------------------------------------
    //  GENERAR CAMPOS CON BINDING + VALIDACIÓN AUTOMÁTICA
    // --------------------------------------------------------

    private void GenerarCampos(object entidad)
    {
        _formularioBuilder.Build(
            entidad,
            spCampos,
            _controles,
            maxPorFila: 3
        );

        FormularioValidator.ForzarValidacionInicial(_controles);
    }

    // -----------------------------
    //   DATA GRID DE DETALLES
    // -----------------------------

    private void CargarTabla()
    {
        if (_detalles != null)
            dgDetalles.ItemsSource = _detalles;
    }

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
            bool ok = await _accion(EntidadEditada);

            if (!ok)
                return;

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