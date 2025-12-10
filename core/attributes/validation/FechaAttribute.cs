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
        // Si ya es DateTime (o Nullable<DateTime> con valor), es válido
        if (value is DateTime)
            return ValidationResult.Success;

        // Permitir vacío (deja que [Required] lo gestione)
        string? texto = value?.ToString();
        if (string.IsNullOrWhiteSpace(texto))
            return ValidationResult.Success;

        if (!EsFechaValida(texto))
        {
            string nombreCampo = validationContext.MemberName ?? "Este campo";
            return new ValidationResult(ErrorMessage ?? $"{nombreCampo} debe tener el formato {Formato}.");
        }

        return ValidationResult.Success;
    }

    private bool EsFechaValida(string fecha)
    {
        return DateTime.TryParseExact(
            fecha,
            Formato,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _
        );
    }
}