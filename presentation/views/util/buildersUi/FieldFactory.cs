using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Gestion.core.attributes;
using Gestion.presentation.enums;
using Gestion.presentation.views.resources.searchbox;
using Gestion.presentation.views.util.buildersUi.data;

namespace Gestion.presentation.views.util.buildersUi;

public static class FieldFactory
{
    // --- Constantes de configuración ---
    private const int FONT_SIZE = 20;

    public static FrameworkElement Crear(
        PropertyInfo prop,
        object entidad,
        ModoFormulario modo)
    {
        var tipo = Nullable.GetUnderlyingType(prop.PropertyType)
                ?? prop.PropertyType;

        var control = CrearControl(prop, entidad, tipo);

        if (EsSoloLectura(prop, modo))
            AplicarSoloLectura(control);

        return control;
    }

    private static FrameworkElement CrearControl(
        PropertyInfo prop,
        object entidad,
        Type tipo)
    {
        // Atributos especiales primero
        if (prop.GetCustomAttribute<SearchableAttribute>() is SearchableAttribute search)
            return CrearSearchBox(prop, entidad, search);


        if (prop.GetCustomAttribute<TextAreaAttribute>() != null)
            return CrearTextArea(prop, entidad);

        if (prop.GetCustomAttribute<ComboSourceAttribute>() is ComboSourceAttribute combo)
            return CrearComboDinamico(prop, entidad, combo);

        if (prop.GetCustomAttribute<RadioGroupAttribute>() is RadioGroupAttribute radioGroup)
            return CrearRadioGroup(prop, entidad, radioGroup);

        // Por tipo
        if (tipo == typeof(bool))
            return CrearCheckBox(prop, entidad);

        if (tipo == typeof(DateTime))
            return CrearDatePicker(prop, entidad);

        // Fallback
        return CrearTextBox(prop, entidad);
    }

    private static bool EsSoloLectura(PropertyInfo prop, ModoFormulario modo)
    {
        return modo == ModoFormulario.SoloLectura
            || prop.GetCustomAttribute<OnlyReadAttribute>() != null;
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
            MaxWidth = 900,
            MinWidth = 300, 
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
            Style = (Style)Application.Current.Resources["BaseTextBoxStyle"]
        }; 

        var widthAttr = prop.GetCustomAttribute<WidthAttribute>();
        if (widthAttr != null)
        {
            tb.Width = widthAttr.Width;
            tb.MinWidth = widthAttr.EffectiveMinWidth;
            tb.MaxWidth = widthAttr.EffectiveMaxWidth;
        }
        
        var binding = BindingFactory.CreateValidateBinding( prop, entidad, BindingMode.TwoWay); 
        
        tb.SetBinding(TextBox.TextProperty, binding); 
        
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

    private static SearchBox CrearSearchBox(
        PropertyInfo prop,
        object entidad,
        SearchableAttribute attr)
    {
        var searchBox = new SearchBox
        {
            Margin = new Thickness(5, 0, 5, 10),
            MinWidth = 200,
            MaxWidth = 300,
            SourceKey = attr.SourceKey
        };

        var binding = BindingFactory.CreateValidateBinding(
            prop,
            entidad,
            BindingMode.TwoWay);

        searchBox.SetBinding(SearchBox.TextProperty, binding);

        return searchBox;
    }

    private static ComboBox CrearComboDinamico(
        PropertyInfo prop,
        object entidad,
        ComboSourceAttribute comboAttr)
    {
        var combo = new ComboBox
        {
            ItemsSource = ComboDataProvider.Get(comboAttr.SourceKey),
            DisplayMemberPath = string.IsNullOrEmpty(comboAttr.Display) ? comboAttr.SourceKey : comboAttr.Display,
            SelectedValuePath = string.IsNullOrEmpty(comboAttr.Value) ? comboAttr.SourceKey : comboAttr.Value,
            Height = 30,
            Width = 300,
            Margin = new Thickness(5, 0, 5, 10),
            FontSize = FONT_SIZE
        };

        var binding = new Binding(prop.Name)
        {
            Source = entidad,
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };

        combo.SetBinding(ComboBox.SelectedValueProperty, binding);

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
                ConverterParameter = valor,
                TargetNullValue = false,
                FallbackValue = false
            };

            rb.SetBinding(RadioButton.IsCheckedProperty, binding);
            panel.Children.Add(rb);
        }

        return panel;
    }

    private static void AplicarSoloLectura(FrameworkElement control)
    {
        switch (control)
        {
            case TextBox tb:
                tb.IsReadOnly = true;
                tb.Background = SystemColors.ControlBrush;
                break;
            case CheckBox cb:
                cb.IsEnabled = false;
                break;
            case DatePicker dp:
                dp.IsEnabled = false;
                break;
            case ComboBox combo:
                combo.IsEnabled = false;
                break;
            case StackPanel panel:
                foreach (var child in panel.Children)
                {
                    if (child is RadioButton rb)
                        rb.IsEnabled = false;
                }
                break;
        }
    }
}