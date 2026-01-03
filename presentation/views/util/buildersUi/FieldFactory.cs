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

    private static TextBox CrearTextArea(PropertyInfo prop, object entidad) 
    { 
        var tb = new TextBox 
        { 
            FontSize = FONT_SIZE, 
            Margin = new Thickness(5, 0, 5, 10), 
            AcceptsReturn = true, 
            TextWrapping = TextWrapping.Wrap, 
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto, 
            MinHeight = FONT_SIZE,
            MaxHeight = FONT_SIZE * 10,
            Width = 900, 
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
            FontSize = FONT_SIZE, 
            Width = 900, 
            Margin = new Thickness(5, 0, 5, 10), 
            TextWrapping = TextWrapping.Wrap, 
            AcceptsReturn = true, 
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto, 
            MinHeight = FONT_SIZE 
        }; 
        
        var binding = BindingFactory.CreateValidateBinding( prop, entidad, BindingMode.TwoWay); 
        
        tb.SetBinding(TextBox.TextProperty, binding); 
        
        return tb; 
    }

    private static TextBlock CrearTextBlock(PropertyInfo prop, object entidad)
    {
        var tb = new TextBlock
        {
            FontSize = FONT_SIZE,
            MaxWidth = 900,
            Margin = new Thickness(5, 0, 5, 10),
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
            Margin = new Thickness(5, 15, 5, 15),
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
            Margin = new Thickness(5, 0, 5, 10),
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
            Margin = new Thickness(5, 0, 5, 10),
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
            Margin = new Thickness(5, 0, 5, 10)
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