using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows.Controls;

namespace Gestion.presentation.utils;

public static class ValidatorProperties
{
    private static readonly Dictionary<Type, Func<ValidationAttribute, string, PropertyInfo, string?>> _validadores =
        new()
        {
            {
                typeof(RequiredAttribute),
                (attr, texto, prop) => string.IsNullOrWhiteSpace(texto) ? 
                ((RequiredAttribute)attr).ErrorMessage ?? $"El campo {prop.Name} es obligatorio.": null
            }
        };

    public static bool Validar(PropertyInfo prop, string texto, TextBox control, out string? mensaje)
    {
        mensaje = null;

        IEnumerable<ValidationAttribute> atributos = prop.GetCustomAttributes<ValidationAttribute>();

        foreach (ValidationAttribute atributo in atributos)
        {
            if (_validadores.TryGetValue(atributo.GetType(), out var validador))
            {
                var error = validador(atributo, texto, prop);
                if (error != null)
                {
                    mensaje = error;
                    return false;
                }
            }
        }

        return true;
    }
}