using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Gestion.presentation.views.util;

public static class BindingFactory
{
    public static Binding CreateValidateBinding(PropertyInfo prop, object source, string? stringFormat = null)
    {
        Binding binding = new(prop.Name)
        {
            Source = source,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Mode = BindingMode.TwoWay,
            ValidatesOnExceptions = true,
            ValidatesOnDataErrors = true,
            ConverterCulture = CultureInfo.GetCultureInfo("es-ES"),
            StringFormat = stringFormat
        };

        DataAnnotationValidationRule rule = new(prop, source)
        {
            ValidationStep = ValidationStep.ConvertedProposedValue
        };

        binding.ValidationRules.Add(rule);

        return binding;
    }
}