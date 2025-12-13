using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Windows.Controls;
using Gestion.core.attributes.validation;
using WPFValidationResult = System.Windows.Controls.ValidationResult;

namespace Gestion.presentation.views.util;

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
            if (value is string s)
            {
                if (string.IsNullOrWhiteSpace(s))
                    valorConvertido = null;
                else if (tipoSubyacente == typeof(DateTime))
                {
                    var fechaAttrLocal = _prop.GetCustomAttribute<FechaAttribute>();
                    var formatoRequerido = fechaAttrLocal?.Formato ?? "dd/MM/yyyy";

                    if (fechaAttrLocal != null)
                    {
                        if (DateTime.TryParseExact(s, 
                            fechaAttrLocal.Formato, 
                            CultureInfo.InvariantCulture, 
                            DateTimeStyles.None, 
                            out var dtExactAttr))
                        {
                            valorConvertido = dtExactAttr;
                        }
                        else
                            return new WPFValidationResult(false, $"{_prop.Name} debe tener el formato {formatoRequerido}.");
                    }
                    else
                    {
                        if (DateTime.TryParseExact(s, 
                            formatoRequerido, 
                            CultureInfo.InvariantCulture, 
                            DateTimeStyles.None, 
                            out var dtExact))
                        {
                            valorConvertido = dtExact;
                        }
                        else if (DateTime.TryParse(s, 
                            cultureInfo, 
                            DateTimeStyles.None, 
                            out var dt))
                        {
                            valorConvertido = dt;
                        }
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
            }
        }
        catch
        {
            return new WPFValidationResult(false, $"{_prop.Name} tiene un formato inválido.");
        }

        var contexto = new ValidationContext(_instancia) { MemberName = _prop.Name };
        var errores = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

        if (!Validator.TryValidateProperty(valorConvertido, contexto, errores))
            return new WPFValidationResult(false, errores[0].ErrorMessage);

        return WPFValidationResult.ValidResult;
    }
}