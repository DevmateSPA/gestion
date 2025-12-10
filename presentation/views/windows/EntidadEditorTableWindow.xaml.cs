using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using Gestion.core.attributes.validation;
using DAValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using WPFValidationResult = System.Windows.Controls.ValidationResult;

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
                var binding = new Binding(prop.Name)
                {
                    Source = EntidadEditada,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true,
                    StringFormat = "dd/MM/yyyy" // formato requerido
                };

                // Validación por DataAnnotations
                var rule = new DataAnnotationValidationRule(prop, EntidadEditada)
                {
                    ValidationStep = ValidationStep.ConvertedProposedValue
                };
                binding.ValidationRules.Add(rule);

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

                // --- BINDING ---
                var binding = new Binding(prop.Name)
                {
                    Source = EntidadEditada,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Mode = BindingMode.TwoWay,
                    ValidatesOnExceptions = true,
                    ValidatesOnDataErrors = true
                };

                // Si existe FechaAttribute pero no es DateTime (caso raro), no aplicamos.
                if (fechaAttr != null && prop.PropertyType == typeof(DateTime))
                {
                    binding.StringFormat = fechaAttr.Formato;
                }

                // Validación por DataAnnotations
                var rule = new DataAnnotationValidationRule(prop, EntidadEditada)
                {
                    ValidationStep = ValidationStep.ConvertedProposedValue
                };
                binding.ValidationRules.Add(rule);

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

        DialogResult = true;
        Close();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}


// ------------------------------------------------------------------
//  VALIDATION RULE COMPATIBLE CON DATAANNOTATIONS (LISTA PARA USAR)
// ------------------------------------------------------------------

public class DataAnnotationValidationRule : ValidationRule
{
    private readonly PropertyInfo _prop;
    private readonly object _instancia;

    public DataAnnotationValidationRule(PropertyInfo prop, object instancia)
    {
        _prop = prop;
        _instancia = instancia;
    }

    public override WPFValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        object? valorConvertido = value;
        Type tipo = _prop.PropertyType;
        Type tipoSubyacente = Nullable.GetUnderlyingType(tipo) ?? tipo;

        try
        {
            // Si el valor llega como cadena y la propiedad espera un tipo concreto,
            // intentamos convertirlo de forma segura (DateTime, enum, primitivos).
            if (value is string s)
            {
                if (string.IsNullOrWhiteSpace(s))
                {
                    valorConvertido = null;
                }
                else if (tipoSubyacente == typeof(DateTime))
                {
                    // Si hay un FechaAttribute, usar su formato para TryParseExact
                    var fechaAttrLocal = _prop.GetCustomAttribute<FechaAttribute>();
                    var formatoRequerido = "dd/MM/yyyy";

                    if (fechaAttrLocal != null)
                    {
                        // preferir formato del atributo si existe
                        if (DateTime.TryParseExact(s, fechaAttrLocal.Formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtExactAttr))
                            valorConvertido = dtExactAttr;
                        else
                            return new WPFValidationResult(false, $"{_prop.Name} debe tener el formato {fechaAttrLocal.Formato}.");
                    }
                    else
                    {
                        // aceptar explícitamente dd/MM/yyyy primero, luego parse flexible según cultureInfo
                        if (DateTime.TryParseExact(s, formatoRequerido, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtExact))
                            valorConvertido = dtExact;
                        else if (DateTime.TryParse(s, cultureInfo, DateTimeStyles.None, out var dt))
                            valorConvertido = dt;
                        else
                            return new WPFValidationResult(false, $"{_prop.Name} debe tener el formato {formatoRequerido}.");
                    }
                }
                else if (tipoSubyacente.IsEnum)
                {
                    try
                    {
                        valorConvertido = Enum.Parse(tipoSubyacente, s, ignoreCase: true);
                    }
                    catch
                    {
                        return new WPFValidationResult(false, $"{_prop.Name} tiene un formato inválido.");
                    }
                }
                else if (tipoSubyacente.IsPrimitive || tipoSubyacente == typeof(decimal))
                {
                    try
                    {
                        valorConvertido = Convert.ChangeType(s, tipoSubyacente, CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        return new WPFValidationResult(false, $"{_prop.Name} tiene un formato inválido.");
                    }
                }
                // otros tipos no convertidos aquí; si WPF ya convirtió, value no será string.
            }
        }
        catch
        {
            return new WPFValidationResult(false,
                $"{_prop.Name} tiene un formato inválido.");
        }

        //--------------------------------------------------------------
        // Validación DataAnnotations final
        //--------------------------------------------------------------
        var contexto = new ValidationContext(_instancia)
        {
            MemberName = _prop.Name
        };

        var errores = new List<DAValidationResult>();

        if (!Validator.TryValidateProperty(valorConvertido, contexto, errores))
            return new WPFValidationResult(false, errores[0].ErrorMessage);

        return WPFValidationResult.ValidResult;
    }

}