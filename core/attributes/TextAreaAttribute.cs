namespace Gestion.core.attributes;

/// <summary>
/// Indica que una propiedad debe renderizarse como un campo de texto multilínea
/// (TextArea) en formularios generados dinámicamente.
/// </summary>
/// <remarks>
/// - El control asociado se renderiza como un <see cref="TextBox"/> multilínea.
/// - Se habilita el salto de línea y el ajuste de texto.
/// - Puede implicar un mayor alto del control.
/// - No afecta validación ni persistencia.
/// </remarks>
/// <example>
/// <code>
/// public class Observacion
/// {
///     [TextArea]
///     public string Comentario { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class TextAreaAttribute : Attribute
{
}