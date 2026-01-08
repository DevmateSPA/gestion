using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;
using Gestion.core.model;
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
    private readonly Dictionary<PropertyInfo, FrameworkElement> _controles = [];

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
    string? propiedadMemo = entidad switch
    {
        Gestion.core.model.Factura => "Memo",
        Gestion.core.model.NotaCredito => "Memo",
        Gestion.core.model.GuiaDespacho => "Memo",
        _ => null
    };

    if (propiedadMemo == null)
        return;

    spMemo.Visibility = Visibility.Visible;

    var binding = new Binding(propiedadMemo)
    {
        Source = EntidadEditada,
        Mode = BindingMode.TwoWay,
        UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
    };

    txtMemo.SetBinding(TextBox.TextProperty, binding);
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
        _formularioBuilder
            .SetEntidad(entidad)
            .SetContenedor(spCampos)
            .SetControles(_controles)
            .SetMaxFila(3)
            .Build();

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


    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        var errores = ValidationHelper.GetValidationErrors(this);
        if (errores.Count != 0)
        {
            DialogUtils.MostrarErroresValidacion(errores);
            return;
        }
        var modal = new ImpresoraModal
        {
            Owner = this 
        };

        if (modal.ShowDialog() == true)
        {
            string impresora = modal.ImpresoraSeleccionada;
            MessageBox.Show("Impresora seleccionada: " + impresora);
            string pdfPath = PrintUtils.GenerarOrdenTrabajoPrint(_entidadOriginal as OrdenTrabajo);
            string sumatra = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SumatraPDF.exe");
            PrintUtils.PrintFile(pdfPath, impresora, sumatra);
        }
       
    }
}
