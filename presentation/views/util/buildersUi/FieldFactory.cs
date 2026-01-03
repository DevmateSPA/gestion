using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Gestion.core.attributes;
using Gestion.presentation.enums;

namespace Gestion.presentation.views.util.buildersUi;

public static class FieldFactory
{
    // --- Constantes de configuración ---
    private const int FONT_SIZE = 20;

    // Text Area CFG
    private static readonly Thickness MARGIN_TEXTAREA = new(5, 0, 5, 10);
    // Text Block CFG
    private static readonly Thickness MARGIN_TEXT_BLOCK = new(5, 0, 5, 10);
    private const double MAX_WIDTH_TEXTBLOCK = 600;
    // Text Box CFG
    private static readonly Thickness MARGIN_TEXT_BOX = new(5, 0, 5, 10);
    // Check Box CFG
    private static readonly Thickness MARGIN_CHECK_BOX = new(5, 6, 5, 10);
    private const double WIDTH_TEXTBOX = 300;
    private const double MIN_HEIGHT_TEXTBOX = 30;
    // Combo Box CFG
    private static readonly Thickness MARGIN_COMBO_BOX = new(5, 0, 5, 10);
    // Date Picker CFG
    private static readonly Thickness MARGIN_DATE_PICKER = new(5, 0, 5, 10);
    // Radio Group CFG
    private static readonly Thickness MARGIN_RADIO_GROUP = new(5, 0, 5, 10);

    public static FrameworkElement Crear(PropertyInfo prop, object entidad, ModoFormulario modo)
    {
        // Solo lectura
        if (prop.GetCustomAttribute<OnlyReadAttribute>() != null || modo == ModoFormulario.SoloLectura)
            return CrearTextBlock(prop, entidad);

        var tipo = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        return tipo switch
        {
            Type t when t == typeof(bool) => CrearCheckBox(prop, entidad),
            Type t when t.IsEnum => CrearCombo(prop, entidad),
            Type t when t == typeof(DateTime) => CrearDatePicker(prop, entidad),
            _ => prop.GetCustomAttribute<TextAreaAttribute>() != null ? CrearTextArea(prop, entidad) :
                prop.GetCustomAttribute<RadioGroupAttribute>() is RadioGroupAttribute radioGroup ? CrearRadioGroup(prop, entidad, radioGroup) :
                CrearTextBox(prop, entidad)
        };
    }

    private static void AplicarEstiloBaseTextBox(TextBox tb, Thickness margin, double? maxWidth = null)
    {
        tb.FontSize = FONT_SIZE;
        tb.Margin = margin;
        tb.TextWrapping = TextWrapping.Wrap;
        tb.AcceptsReturn = true;
        tb.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        tb.HorizontalAlignment = HorizontalAlignment.Stretch;
        tb.MinHeight = FONT_SIZE;
        if (maxWidth.HasValue) tb.MaxWidth = maxWidth.Value;
    }

    private static TextBox CrearTextArea(PropertyInfo prop, object entidad) 
    { 
        var tb = new TextBox 
        { 
            FontSize = FONT_SIZE, 
            Margin = MARGIN_TEXTAREA, 
            AcceptsReturn = true, 
            TextWrapping = TextWrapping.Wrap, 
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto, 
            MinHeight = FONT_SIZE,
            MaxHeight = FONT_SIZE * 10,
            Width = 1000, 
            HorizontalAlignment = HorizontalAlignment.Stretch 
        }; 
        
        var binding = BindingFactory.CreateValidateBinding( prop, entidad, BindingMode.TwoWay); 
        
        tb.SetBinding(TextBox.TextProperty, binding); 
        
        return tb; 
    }

    private static TextBox CrearTextBox(PropertyInfo prop, object entidad)
    {
        var tb = new TextBox
        {
            Width = WIDTH_TEXTBOX
        };
        AplicarEstiloBaseTextBox(tb, MARGIN_TEXT_BOX);

        var binding = BindingFactory.CreateValidateBinding(prop, entidad, BindingMode.TwoWay);
        tb.SetBinding(TextBox.TextProperty, binding);

        return tb;
    }

    private static TextBlock CrearTextBlock(PropertyInfo prop, object entidad)
    {
        var tb = new TextBlock
        {
            FontSize = FONT_SIZE,
            MaxWidth = MAX_WIDTH_TEXTBLOCK,
            Margin = MARGIN_TEXT_BLOCK,
            VerticalAlignment = VerticalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };

        var binding = BindingFactory.CreateValidateBinding(prop, entidad, BindingMode.OneWay);
        var tipo = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (tipo == typeof(DateTime))
            binding.StringFormat = "dd/MM/yyyy";

        tb.SetBinding(TextBlock.TextProperty, binding);
        return tb;
    }

    private static CheckBox CrearCheckBox(PropertyInfo prop, object entidad)
    {
        var cb = new CheckBox
        {
            Margin = MARGIN_CHECK_BOX,
            FontSize = FONT_SIZE
        };

        var binding = BindingFactory.CreateValidateBinding(
            prop, 
            entidad,
            BindingMode.TwoWay);
        cb.SetBinding(CheckBox.IsCheckedProperty, binding);

        return cb;
    }

    private static ComboBox CrearCombo(PropertyInfo prop, object entidad)
    {
        var combo = new ComboBox
        {
            ItemsSource = Enum.GetValues(
                Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType),
            Height = 30,
            Width = 300,
            Margin = MARGIN_COMBO_BOX,
            FontSize = FONT_SIZE
        };

        var binding = BindingFactory.CreateValidateBinding(
            prop, 
            entidad,
            BindingMode.TwoWay);
        combo.SetBinding(ComboBox.SelectedItemProperty, binding);

        return combo;
    }

    private static DatePicker CrearDatePicker(PropertyInfo prop, object entidad)
    {
        var dp = new DatePicker
        {
            FontSize = FONT_SIZE,
            Height = 30,
            Width = 300,
            Margin = MARGIN_DATE_PICKER,
            SelectedDateFormat = DatePickerFormat.Short,
            Language = XmlLanguage.GetLanguage("es-ES"),
            Text = "Seleccione fecha"
        };

        var binding = BindingFactory.CreateValidateBinding(
            prop, 
            entidad, 
            BindingMode.TwoWay,
            "dd/MM/yyyy");
        dp.SetBinding(DatePicker.SelectedDateProperty, binding);

        return dp;
    }

    private static StackPanel CrearRadioGroup(PropertyInfo prop, object entidad, RadioGroupAttribute radioGroup)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = MARGIN_RADIO_GROUP
        };

        foreach (var (texto, valor) in radioGroup.Opciones)
        {
            var rb = new RadioButton
            {
                Content = texto,
                GroupName = prop.Name,
                Margin = new Thickness(0, 0, 50, 0),
                FontSize = FONT_SIZE
            };

            // Binding con conversor: RadioEqualsConverter
            var binding = new Binding(prop.Name)
            {
                Source = entidad,
                Mode = BindingMode.TwoWay,
                Converter = (IValueConverter)Application.Current.Resources["RadioEqualsConverter"]!,
                ConverterParameter = valor
            };

            rb.SetBinding(RadioButton.IsCheckedProperty, binding);
            panel.Children.Add(rb);
        }

        return panel;
    }
}