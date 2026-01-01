using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Gestion.core.attributes;

namespace Gestion.presentation.views.util.buildersUi;

public static class FieldFactory
{
    public static FrameworkElement Crear(PropertyInfo prop, object entidad)
    {
        if (prop.GetCustomAttribute<OnlyReadAttribute>() != null)
            return CrearTextBlock(prop, entidad);

        var tipo = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        if (tipo == typeof(bool))
            return CrearCheckBox(prop, entidad);

        if (tipo.IsEnum)
            return CrearCombo(prop, entidad);

        if (tipo == typeof(DateTime))
            return CrearDatePicker(prop, entidad);

        return CrearTextBox(prop, entidad);
    }

    private static TextBlock CrearTextBlock(PropertyInfo prop, object entidad)
    {
        var tb = new TextBlock
        {
            FontSize = 20,
            Height = 30,
            Width = 300,
            Margin = new Thickness(5, 6, 5, 10),
            VerticalAlignment = VerticalAlignment.Center
        };

        var binding = BindingFactory.CreateValidateBinding(prop, entidad);
        tb.SetBinding(TextBlock.TextProperty, binding);

        return tb;
    }

    private static TextBox CrearTextBox(PropertyInfo prop, object entidad)
    {
        var tb = new TextBox
        {
            FontSize = 20,
            Height = 30,
            Width = 300,
            Margin = new Thickness(5, 0, 5, 10)
        };

        var binding = BindingFactory.CreateValidateBinding(prop, entidad);
        tb.SetBinding(TextBox.TextProperty, binding);

        return tb;
    }

    private static CheckBox CrearCheckBox(PropertyInfo prop, object entidad)
    {
        var cb = new CheckBox
        {
            Margin = new Thickness(5, 6, 5, 10)
        };

        var binding = BindingFactory.CreateValidateBinding(prop, entidad);
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
            Margin = new Thickness(5, 0, 5, 10)
        };

        var binding = BindingFactory.CreateValidateBinding(prop, entidad);
        combo.SetBinding(ComboBox.SelectedItemProperty, binding);

        return combo;
    }

    private static DatePicker CrearDatePicker(PropertyInfo prop, object entidad)
    {
        var dp = new DatePicker
        {
            FontSize = 20,
            Height = 30,
            Width = 300,
            Margin = new Thickness(5, 0, 5, 10),
            SelectedDateFormat = DatePickerFormat.Short,
            Language = XmlLanguage.GetLanguage("es-ES")
        };

        var binding = BindingFactory.CreateValidateBinding(prop, entidad, "dd/MM/yyyy");
        dp.SetBinding(DatePicker.SelectedDateProperty, binding);

        return dp;
    }
}
