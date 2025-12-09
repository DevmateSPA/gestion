using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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

        // Focus al primer campo
        if (spCampos.Children.OfType<StackPanel>()
                .SelectMany(x => x.Children.OfType<StackPanel>())
                .SelectMany(x => x.Children.OfType<TextBox>())
                .FirstOrDefault() is TextBox primerCampo)
        {
            primerCampo.Focus();
        }

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

            // TextBox con binding
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

            // Formato de fecha si tiene atributo Fecha
            var fechaAttr = prop.GetCustomAttribute<FechaAttribute>();
            if (fechaAttr != null && prop.PropertyType == typeof(DateTime))
            {
                binding.StringFormat = fechaAttr.Formato;
            }

            // Validación por DataAnnotations
            var rule = new DataAnnotationValidationRule(prop, EntidadEditada)
            {
                // Validar después de que el binding convierta la cadena al tipo de la propiedad
                ValidationStep = ValidationStep.ConvertedProposedValue
            };
            binding.ValidationRules.Add(rule);

            textBox.SetBinding(TextBox.TextProperty, binding);

            // Bloque del campo
            var bloque = new StackPanel { Orientation = Orientation.Vertical, Width = 310 };
            bloque.Children.Add(label);
            bloque.Children.Add(textBox);

            if (i % maxPorFila == 0)
            {
                filaActual = new StackPanel { Orientation = Orientation.Horizontal };
                spCampos.Children.Add(filaActual);
            }

            filaActual?.Children.Add(bloque);
        }

        // Forzar validación inicial en todos los TextBox
        foreach (var tb in spCampos
            .Children.OfType<StackPanel>()
            .SelectMany(f => f.Children.OfType<StackPanel>())
            .SelectMany(c => c.Children.OfType<TextBox>()))
        {
            tb.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
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
        // Buscar errores en todos los TextBox generados
        bool hayErrores = spCampos
            .Children
            .OfType<StackPanel>()
            .SelectMany(f => f.Children.OfType<StackPanel>())
            .SelectMany(c => c.Children.OfType<TextBox>())
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
                    if (fechaAttrLocal != null)
                    {
                        if (DateTime.TryParseExact(s, fechaAttrLocal.Formato, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtExact))
                            valorConvertido = dtExact;
                        else
                            return new WPFValidationResult(false, $"{_prop.Name} debe tener el formato {fechaAttrLocal.Formato}.");
                    }
                    else
                    {
                        // Sin atributo, intento parse flexible según cultureInfo
                        if (DateTime.TryParse(s, cultureInfo, DateTimeStyles.None, out var dt))
                            valorConvertido = dt;
                        else
                            return new WPFValidationResult(false, $"{_prop.Name} tiene un formato inválido.");
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