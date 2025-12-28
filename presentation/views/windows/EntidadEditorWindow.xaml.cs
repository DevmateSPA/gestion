using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;
using Gestion.helpers;
using Gestion.presentation.views.util;
using Gestion.presentation.views.util.buildersUi;

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorWindow : Window
{
    // Builder de Ui
    private readonly FormularioBuilder _formularioBuilder = new();
    private IModel _entidadOriginal;
    // Cambiado para almacenar TextBox o DatePicker
    private readonly Func<IModel, Task<bool>>? _accion;
    private readonly Dictionary<PropertyInfo, Control> _controles = [];

    public IModel EntidadEditada { get; private set; }

    public EntidadEditorWindow(IModel entidad, Func<IModel, Task<bool>>? accion = null, string titulo = "Ventana")
    {
        InitializeComponent();
        Title = titulo;

        _accion = accion;

        InicializarEntidad(entidad);
        InicializarUI(entidad);
        InicializarEventos();
    }

    private void InicializarEntidad(IModel entidad)
    {
        _entidadOriginal = entidad;

        EntidadEditada = (IModel)Activator.CreateInstance(entidad.GetType())!;

        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }
    }

    private void InicializarUI(object entidad)
    {
        GenerarCampos(EntidadEditada);
        CargarMemoSiAplica(entidad);
        EnfocarPrimerControl();
    }

    private void CargarMemoSiAplica(object entidad)
    {
        switch (entidad)
        {
            case Gestion.core.model.Factura f:
                spMemo.Visibility = Visibility.Visible;
                txtMemo.Text = f.Memo;
                break;

            case Gestion.core.model.NotaCredito nc:
                spMemo.Visibility = Visibility.Visible;
                txtMemo.Text = nc.Memo;
                break;

            case Gestion.core.model.GuiaDespacho gd:
                spMemo.Visibility = Visibility.Visible;
                txtMemo.Text = gd.Memo;
                break;
        }
    }

    private void EnfocarPrimerControl()
    {
        var primerControl = _controles.Values.FirstOrDefault();
        primerControl?.Focus();
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

    private void GenerarCampos(object entidad)
    {
        _formularioBuilder.Build(
            entidad: entidad,
            contenedor: spCampos,
            controles: _controles,
            maxPorFila: 3
        );

        FormularioValidator.ForzarValidacionInicial(_controles);
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
