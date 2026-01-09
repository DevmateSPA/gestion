using System.ComponentModel.DataAnnotations;

namespace Gestion.core.attributes.validation;

/// <summary>
/// Valida que un valor sea un RUT chileno válido y obligatorio.
/// </summary>
/// <remarks>
/// - El valor no puede ser nulo, vacío ni contener solo espacios.
/// - Valida formato y dígito verificador del RUT chileno.
/// - Acepta RUT con o sin puntos y guión.
/// - Este atributo incluye la validación de obligatoriedad, por lo que
///   no es necesario usar <see cref="RequiredAttribute"/> junto a él.
/// </remarks>
/// <example>
/// <code>
/// [Rut]
/// public string Rut { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
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
    

    /// <summary>
    /// Determina si un RUT chileno es válido según su dígito verificador.
    /// </summary>
    /// <param name="rut">RUT a validar.</param>
    /// <returns>
    /// <c>true</c> si el RUT es válido; de lo contrario, <c>false</c>.
    /// </returns>
    private static bool EsRutValido(string rut)
    {
        rut = rut.Trim()
                .Replace(".", "")
                .Replace("-", "")
                .ToUpper();

        if (rut.Length < 2)
            return false;

        string cuerpo = rut[..^1];
        char dv = rut[^1];

        if (!int.TryParse(cuerpo, out _))
            return false;

        int suma = 0;
        int multiplicador = 2;

        for (int i = cuerpo.Length - 1; i >= 0; i--)
        {
            suma += (cuerpo[i] - '0') * multiplicador;
            multiplicador = multiplicador == 7 ? 2 : multiplicador + 1;
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