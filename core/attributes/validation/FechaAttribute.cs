using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Gestion.core.attributes.validation;


/// <summary>
/// No se usa quedo de legado, antes de usar DatePickers dinamicos
/// </summary>
public class FechaAttribute : ValidationAttribute
{
    public string Formato { get; }

    public FechaAttribute(string formato = "dd/MM/yyyy")
    {
        Formato = formato;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // null es válido (Required se encarga)
        if (value is null)
            return ValidationResult.Success;

        // DateTime o DateTime? con valor
        if (value is DateTime)
            return ValidationResult.Success;

        return new ValidationResult("Debe ingresar una fecha válida.");
    }
}