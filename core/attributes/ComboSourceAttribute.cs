namespace Gestion.core.attributes;

/// <summary>
/// Define la fuente de datos para un <see cref="ComboBox"/> generado dinámicamente.
/// </summary>
/// <remarks>
/// - <see cref="SourceKey"/> identifica la fuente de datos registrada en el sistema.
/// - <see cref="Display"/> indica la propiedad a mostrar al usuario.
/// - <see cref="Value"/> indica la propiedad que se utilizará como valor interno.
/// - Si <see cref="Display"/> o <see cref="Value"/> no se especifican,
///   se utilizará el comportamiento por defecto del sistema.
/// </remarks>
/// <example>
/// <code>
/// [ComboSource("Estados")]
/// public int EstadoId { get; set; }
///
/// [ComboSource("Usuarios", Display = "Nombre", Value = "Id")]
/// public int UsuarioId { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ComboSourceAttribute : Attribute
{
    public string SourceKey { get; }
    public string Display { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public ComboSourceAttribute(string sourceKey)
    {
        SourceKey = sourceKey;
    }
}