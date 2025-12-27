using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Gestion.core.attributes.validation;
using Gestion.core.interfaces.model;
using Gestion.helpers;
using Gestion.presentation.views.util;

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorWindow : Window
{
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
        var tipo = entidad.GetType();

        // Seleccionar propiedades visibles
        var propiedades = tipo.GetProperties()
            .Where(p =>
                p.CanWrite &&
                p.Name != "Memo" &&
                !string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) &&
                (p.PropertyType == typeof(string) || p.PropertyType.IsValueType) &&
                (p.GetCustomAttribute<VisibleAttribute>()?.Mostrar ?? true)
            )
            .ToList();

        spCampos.Children.Clear();
        _controles.Clear();

        int maxPorFila = 3;
        StackPanel? filaActual = null;

        for (int i = 0; i < propiedades.Count; i++)
        {
            var prop = propiedades[i];

            var label = new TextBlock
            {
                Text = prop.GetCustomAttribute<NombreAttribute>()?.Texto ?? prop.Name,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 4, 0, 2),
                TextWrapping = TextWrapping.Wrap
            };

            var valor = prop.GetValue(entidad);

            var fechaAttr = prop.GetCustomAttribute<FechaAttribute>();

            // Detectar DateTime o DateTime?
            var tipoSubyacente = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
            bool esDateTime = tipoSubyacente == typeof(DateTime);

            Control control;

            if (esDateTime)
            {
                // DatePicker para DateTime
                var dp = new DatePicker
                {
                    FontSize = 20,
                    Height = 30,
                    Width = 300,
                    Margin = new Thickness(5, 0, 5, 10),
                    SelectedDateFormat = DatePickerFormat.Short,
                    Language = XmlLanguage.GetLanguage("es-ES")
                };

                // --- BINDING al SelectedDate ---
                var binding = BindingFactory.CreateValidateBinding(prop, EntidadEditada, "dd/MM/yyyy");

                dp.SetBinding(DatePicker.SelectedDateProperty, binding);

                control = dp;
            }
            else
            {
                string valorTexto = valor?.ToString() ?? "";

                if (fechaAttr != null && valor != null && valor is DateTime fecha)
                {
                    valorTexto = fecha.ToString(fechaAttr.Formato);
                }

                var textBox = new TextBox
                {
                    Text = valorTexto,
                    FontSize = 20,
                    Height = 30,
                    Width = 300,
                    Margin = new Thickness(5, 0, 5, 10)
                };

                // --- BINDING al Text ---
                var binding = BindingFactory.CreateValidateBinding(prop, EntidadEditada);

                textBox.SetBinding(TextBox.TextProperty, binding);

                control = textBox;
            }

            _controles[prop] = control;

            var bloque = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Width = 310
            };

            bloque.Children.Add(label);
            bloque.Children.Add(control);

            if (i % maxPorFila == 0)
            {
                filaActual = new StackPanel { Orientation = Orientation.Horizontal };
                spCampos.Children.Add(filaActual);
            }

            filaActual?.Children.Add(bloque);
        }

        // Forzar validación inicial en todos los controles generados (TextBox y DatePicker)
        foreach (var ctrl in spCampos
            .Children.OfType<StackPanel>()
            .SelectMany(f => f.Children.OfType<StackPanel>())
            .SelectMany(c => c.Children.OfType<Control>().Where(c => c is TextBox || c is DatePicker)))
        {
            if (ctrl is TextBox tb)
                tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            else if (ctrl is DatePicker dp)
                dp.GetBindingExpression(DatePicker.SelectedDateProperty)?.UpdateSource();
        }
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
