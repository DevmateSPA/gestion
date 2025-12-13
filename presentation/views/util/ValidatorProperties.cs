using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Gestion.presentation.views.util;

public static class ValidatorProperties
{
    public static bool Validar(PropertyInfo prop, object valor, out string? mensaje)
    {
        mensaje = null;

        var atributos = prop.GetCustomAttributes<ValidationAttribute>();

        foreach (var atributo in atributos)
        {
            var contexto = new ValidationContext(valor)
            {
                MemberName = prop.Name
            };

            var resultado = atributo.GetValidationResult(valor, contexto);

            if (resultado != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            {
                mensaje = resultado?.ErrorMessage;
                return false;
            }
        }

        return true;
    }
}