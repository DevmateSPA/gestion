using System;
using System.Globalization;
using System.Windows.Data;

namespace Gestion.presentation.views.util;

public class RadioEqualsConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() == parameter?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return parameter?.ToString() ?? "1"; // devuelve "1", "2" o "3"
        return Binding.DoNothing;
    }
}
