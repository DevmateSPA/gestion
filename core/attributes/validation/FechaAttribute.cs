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
        string? texto = value?.ToString();

        // Permitir vacío (deja que [Required] controle eso)
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
