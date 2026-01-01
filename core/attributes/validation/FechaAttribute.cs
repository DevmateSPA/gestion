using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Gestion.core.attributes.validation;

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