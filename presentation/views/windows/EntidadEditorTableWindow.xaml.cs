using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    private readonly object _entidadOriginal;
    private Dictionary<PropertyInfo, FrameworkElement> _controles = [];

    private DataGrid? dgDetalles;

    public object? EntidadEditada { get; private set; }

    private readonly Func<object, Task<bool>>? _guardar;
    private readonly Action<OrdenTrabajo>? _imprimir;

    public EntidadEditorTableWindow(
        object entidad,
        Func<object, Task<bool>> guardar,
        Action<OrdenTrabajo>? imprimir,
        ModoFormulario modo,
        bool shouldImprimir,
        string titulo = "Ventana con tabla")
    {
        InitializeComponent();
        Title = titulo;

        _entidadOriginal = entidad;
        _guardar = guardar;
        _imprimir = imprimir;

        ClonarEntidad(entidad);

        InicializarUI(EntidadEditada!, modo, shouldImprimir);
        InicializarEventos();
    }

    private void ClonarEntidad(object entidad)
    {
        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;

        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }
    }

    private void InicializarUI(object entidad, ModoFormulario modo, bool shouldImprimir)
    {
        // Crear el builder para la entidad
        var builder = new VentanaBuilder<object>()
            .SetEntidad(entidad)
            .SetContenedorCampos(spCampos)
            .SetContenedorTablas(spTabla)
            .SetBotonGuardar(btnGuardar)
            .SetBotonImprimir(btnImprimir, shouldImprimir)
            .SetModo(modo);

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

    private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        if (!Validar())
            return;


        if (_guardar == null)
            throw new InvalidOperationException("Función de guardado no proporcionada.");
            
        var ok = await _guardar(EntidadEditada!);

        if (!ok)
            return;

        DialogResult = true;
    }

    private async void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        if (!Validar())
            return;

        if (_guardar != null)
            await _guardar(EntidadEditada!);

        if (_imprimir == null)
            throw new InvalidOperationException("Función de impresión no proporcionada.");

        _imprimir((OrdenTrabajo)EntidadEditada!);
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}