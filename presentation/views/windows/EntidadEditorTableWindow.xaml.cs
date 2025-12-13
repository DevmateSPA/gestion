using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Gestion.core.attributes.validation;
using Gestion.presentation.views.util;

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorTableWindow : Window
{
    private readonly object _entidadOriginal;
    private readonly IEnumerable<object>? _detalles;

    public object EntidadEditada { get; private set; }

    public EntidadEditorTableWindow(Page padre, object entidad, IEnumerable<object>? detalles, string titulo = "Ventana con tabla")
    {
        InitializeComponent();
        this.Owner = Window.GetWindow(padre);
        Title = titulo;

        _entidadOriginal = entidad;
        _detalles = detalles;

        // Clonar la entidad original
        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }

        GenerarCampos();
        CargarTabla();

        // Focus al primer campo (TextBox o DatePicker)
        var primerControl = spCampos.Children.OfType<StackPanel>()
                .SelectMany(x => x.Children.OfType<StackPanel>())
                .SelectMany(x => x.Children.OfType<Control>().Where(c => c is TextBox || c is DatePicker))
                .FirstOrDefault();
        primerControl?.Focus();

        this.PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        };
    }

    // --------------------------------------------------------
    //  GENERAR CAMPOS CON BINDING + VALIDACIÓN AUTOMÁTICA
    // --------------------------------------------------------

    private void GenerarCampos()
    {
        var props = EntidadEditada.GetType()
            .GetProperties()
            .Where(p =>
                p.CanWrite &&
                !string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) &&
                (p.PropertyType == typeof(string) || p.PropertyType.IsValueType) &&
                (p.GetCustomAttribute<VisibleAttribute>()?.Mostrar ?? true)
            )
            .ToList();

        spCampos.Children.Clear();

        int maxPorFila = 3;
        StackPanel? filaActual = null;

        for (int i = 0; i < props.Count; i++)
        {
            var prop = props[i];

            // Etiqueta
            var label = new TextBlock
            {
                Text = prop.GetCustomAttribute<NombreAttribute>()?.Texto ?? prop.Name,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 4, 0, 2)
            };

            // Determinar si es fecha
            var fechaAttr = prop.GetCustomAttribute<FechaAttribute>();
            bool esDateTime = prop.PropertyType == typeof(DateTime);

            // Controles por defecto
            Control controlCampo;

            if (esDateTime)
            {
                // DatePicker para DateTime, forzando formato dd/MM/yyyy
                var datePicker = new DatePicker
                {
                    FontSize = 20,
                    Height = 30,
                    Width = 300,
                    Margin = new Thickness(5, 0, 5, 10),
                    SelectedDateFormat = DatePickerFormat.Short,
                    Language = XmlLanguage.GetLanguage("es-ES")
                };

                // --- BINDING sobre SelectedDate ---
                var binding = BindingFactory.CreateValidateBinding(prop, EntidadEditada, "dd/MM/yyyy");

                datePicker.SetBinding(DatePicker.SelectedDateProperty, binding);
                controlCampo = datePicker;
            }
            else
            {
                // TextBox para el resto
                var textBox = new TextBox
                {
                    FontSize = 20,
                    Height = 30,
                    Width = 300,
                    Margin = new Thickness(5, 0, 5, 10)
                };

                // Si existe FechaAttribute pero no es DateTime (caso raro), no aplicamos.

                // --- BINDING ---
                var binding = BindingFactory.CreateValidateBinding(prop, EntidadEditada);

                if (fechaAttr != null && prop.PropertyType == typeof(DateTime))
                {
                    binding.StringFormat = fechaAttr.Formato;
                }

                textBox.SetBinding(TextBox.TextProperty, binding);
                controlCampo = textBox;
            }

            // Bloque del campo
            var bloque = new StackPanel { Orientation = Orientation.Vertical, Width = 310 };
            bloque.Children.Add(label);
            bloque.Children.Add(controlCampo);

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

    // -----------------------------
    //   DATA GRID DE DETALLES
    // -----------------------------

    private void CargarTabla()
    {
        if (_detalles != null)
            dgDetalles.ItemsSource = _detalles;
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        var errores = ValidationHelper.GetValidationErrors(spCampos);

        if (errores.Count != 0)
        {
            DialogUtils.MostrarErroresValidacion(errores);
            return;
        }

        DialogResult = true;
        Close();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}