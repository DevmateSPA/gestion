using System.ComponentModel.DataAnnotations;

namespace Gestion.core.attributes.validation;

public class RutAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string? texto = value?.ToString();

        if (string.IsNullOrWhiteSpace(texto))
            return new ValidationResult(ErrorMessage ?? "El RUT es obligatorio.");

        if (!EsRutValido(texto))
            return new ValidationResult(ErrorMessage ?? "El RUT ingresado no es válido.");

        return ValidationResult.Success;
    }
    
    private static bool EsRutValido(string rut)
    {
            rut = rut.Trim()
                .Replace(".", "")
                .Replace("-", "")
                .ToUpper();

        string cuerpo = rut[..^1];
        char dv = rut[^1];

        if (!int.TryParse(cuerpo, out _))
            return false;

        int suma = 0;
        int multiplicador = 2;

        for (int i = cuerpo.Length - 1; i >= 0; i--)
        {
            suma += (cuerpo[i] - '0') * multiplicador;
            multiplicador = (multiplicador == 7) ? 2 : multiplicador + 1;
        }

        int resto = 11 - (suma % 11);

        char dvEsperado = resto switch
        {
            11 => '0',
            10 => 'K',
            _  => resto.ToString()[0]
        };

        return dv == dvEsperado;
    }
}