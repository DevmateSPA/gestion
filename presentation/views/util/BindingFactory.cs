using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;

namespace Gestion.presentation.views.util;

/// <summary>
/// Fábrica de bindings WPF con soporte de validación
/// basada en Data Annotations.
/// </summary>
/// <remarks>
/// Esta clase centraliza la creación de <see cref="Binding"/> para:
/// <list type="bullet">
/// <item>Evitar duplicación de configuración.</item>
/// <item>Habilitar validaciones automáticas.</item>
/// <item>Configurar cultura y formato de datos.</item>
/// </list>
/// Está pensada para usarse en formularios dinámicos
/// y generación automática de controles.
/// </remarks>
public static class BindingFactory
{
    /// <summary>
    /// Crea un <see cref="Binding"/> con validación basada en
    /// Data Annotations para una propiedad específica.
    /// </summary>
    /// <param name="prop">
    /// Propiedad del objeto origen a la que se enlaza el binding.
    /// </param>
    /// <param name="source">
    /// Instancia del objeto que actúa como origen de datos.
    /// </param>
    /// <param name="mode">
    /// Modo del binding (OneWay, TwoWay, etc.).
    /// </param>
    /// <param name="stringFormat">
    /// Formato opcional para la conversión a string
    /// (por ejemplo, fechas o números).
    /// </param>
    /// <returns>
    /// Un <see cref="Binding"/> configurado con validación
    /// y reglas de conversión apropiadas.
    /// </returns>
    /// <remarks>
    /// Si el modo es <see cref="BindingMode.TwoWay"/>, se agrega
    /// una regla de validación basada en <see cref="DataAnnotationValidationRule"/>,
    /// evaluada en la etapa <see cref="ValidationStep.ConvertedProposedValue"/>.
    /// </remarks>
    public static Binding CreateValidateBinding(
        PropertyInfo prop,
        object source,
        BindingMode mode,
        string? stringFormat = null)
    {
        Binding binding = new(prop.Name)
        {
            Source = source,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            Mode = mode,
            ValidatesOnExceptions = true,
            ValidatesOnDataErrors = true,
            ConverterCulture = CultureInfo.GetCultureInfo("es-ES"),
        };

        if (!string.IsNullOrWhiteSpace(stringFormat))
            binding.StringFormat = stringFormat;

        if (mode == BindingMode.TwoWay)
        {
            DataAnnotationValidationRule rule = new(prop, source)
            {
                ValidationStep = ValidationStep.ConvertedProposedValue
            };
            binding.ValidationRules.Add(rule);
        }

        return binding;
    }
}