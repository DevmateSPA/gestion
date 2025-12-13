using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Gestion.core.attributes.validation;
using Gestion.presentation.views.util;

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorWindow : Window
{
    private readonly object _entidadOriginal;
    // Cambiado para almacenar TextBox o DatePicker
    private readonly Dictionary<PropertyInfo, Control> _controles = new();

    public object EntidadEditada { get; private set; }

    public EntidadEditorWindow(Page padre, object entidad, string titulo = "Ventana")
    {
        InitializeComponent();
        this.Owner = Window.GetWindow(padre);
        Title = titulo;

        _entidadOriginal = entidad;

        // Clonar entidad
        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }

        GenerarCampos(EntidadEditada);

        // Cargar MEMO si aplica
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

        // Focus al primer control (TextBox o DatePicker)
        var primerControl = _controles.Values.FirstOrDefault();
        primerControl?.Focus();

        // ESC para cerrar
        this.PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
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
                var formato = fechaAttr?.Formato ?? "dd/MM/yyyy";

                var binding = new Binding(prop.Name)
                {
                    Source = EntidadEditada,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true,
                    StringFormat = formato,
                    ConverterCulture = CultureInfo.GetCultureInfo("es-ES")
                };

                var rule = new DataAnnotationValidationRule(prop, EntidadEditada)
                {
                    ValidationStep = ValidationStep.ConvertedProposedValue
                };
                binding.ValidationRules.Add(rule);

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
                var binding = new Binding(prop.Name)
                {
                    Source = EntidadEditada,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true,
                    ConverterCulture = CultureInfo.GetCultureInfo("es-ES")
                };

                var rule = new DataAnnotationValidationRule(prop, EntidadEditada)
                {
                    ValidationStep = ValidationStep.ConvertedProposedValue
                };
                binding.ValidationRules.Add(rule);

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

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        // Buscar errores en todos los controles generados (TextBox y DatePicker)
        bool hayErrores = spCampos
            .Children
            .OfType<StackPanel>()
            .SelectMany(f => f.Children.OfType<StackPanel>())
            .SelectMany(c => c.Children.OfType<Control>().Where(c => c is TextBox || c is DatePicker))
            .Any(t => Validation.GetHasError(t));

        if (hayErrores)
        {
            MessageBox.Show(
                "Hay errores en el formulario. Corrígelos antes de guardar.",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
            return;
        }

        // Al usar binding TwoWay con Source=EntidadEditada, los valores ya están actualizados.
        DialogResult = true;
        Close();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
