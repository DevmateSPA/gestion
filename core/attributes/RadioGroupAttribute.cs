namespace Gestion.core.attributes;

/// <summary>
/// Define un grupo de opciones para renderizar un conjunto de RadioButton
/// en formularios generados dinámicamente.
/// </summary>
/// <remarks>
/// - Las opciones se definen como pares Texto / Valor.
/// - El número de parámetros debe ser par.
/// - El valor se asigna a la propiedad asociada.
/// </remarks>
/// <example>
/// <code>
/// [RadioGroup("Activo", "A", "Inactivo", "I")]
/// public string Estado { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class RadioGroupAttribute : Attribute
{
    /// <summary>
    /// Opciones disponibles del grupo de radio botones.
    /// </summary>
    public (string Texto, string Valor)[] Opciones { get; }

    /// <summary>
    /// Inicializa el grupo de opciones.
    /// </summary>
    /// <param name="opciones">
    /// Pares Texto / Valor (cantidad par obligatoria).
    /// </param>
    public RadioGroupAttribute(params object[] opciones)
    {
        if (opciones.Length == 0 || opciones.Length % 2 != 0)
            throw new ArgumentException(
                "Las opciones deben definirse como pares Texto / Valor.");

        var lista = new List<(string Texto, string Valor)>();

        for (int i = 0; i < opciones.Length; i += 2)
        {
            lista.Add((
                opciones[i]?.ToString() ?? string.Empty,
                opciones[i + 1]?.ToString() ?? string.Empty
            ));
        }

        Opciones = [.. lista];
    }
}