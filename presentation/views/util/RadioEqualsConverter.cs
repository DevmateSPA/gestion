using System;
using System.Globalization;
using System.Windows.Data;

namespace Gestion.presentation.views.util;

/// <summary>
/// Convertidor de valores para enlazar grupos de <see cref="System.Windows.Controls.RadioButton"/>
/// a una propiedad de tipo string u objeto convertible a string.
/// </summary>
/// <remarks>
/// Este convertidor permite:
/// <list type="bullet">
/// <item>Marcar un RadioButton como seleccionado cuando su valor coincide con la propiedad enlazada.</item>
/// <item>Actualizar la propiedad enlazada cuando el RadioButton es seleccionado.</item>
/// </list>
/// Es especialmente útil en formularios dinámicos donde las opciones
/// se definen mediante atributos como <c>RadioGroupAttribute</c>.
/// </remarks>
public class RadioEqualsConverter : IValueConverter
{
    /// <summary>
    /// Convierte el valor del modelo al estado de selección del RadioButton.
    /// </summary>
    /// <param name="value">Valor actual de la propiedad enlazada.</param>
    /// <param name="targetType">Tipo del destino (normalmente <see cref="bool"/>).</param>
    /// <param name="parameter">
    /// Valor asociado al RadioButton que se compara con la propiedad.
    /// </param>
    /// <param name="culture">Cultura utilizada en la conversión.</param>
    /// <returns>
    /// <c>true</c> si el valor de la propiedad coincide con el parámetro,
    /// en caso contrario <c>false</c>.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        return value.ToString() == parameter.ToString();
    }

    /// <summary>
    /// Convierte el estado de selección del RadioButton al valor del modelo.
    /// </summary>
    /// <param name="value">Estado de selección del RadioButton.</param>
    /// <param name="targetType">Tipo del valor de retorno.</param>
    /// <param name="parameter">
    /// Valor asociado al RadioButton seleccionado.
    /// </param>
    /// <param name="culture">Cultura utilizada en la conversión.</param>
    /// <returns>
    /// El valor del parámetro si el RadioButton está seleccionado,
    /// o <see cref="Binding.DoNothing"/> si no lo está.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isChecked && isChecked)
            return parameter?.ToString();

        return Binding.DoNothing;
    }
}
