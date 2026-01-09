namespace Gestion.core.attributes;

/// <summary>
/// Define el ancho preferido de un TextBox (No afecta a TextArea) generado dinámicamente.
/// </summary>
/// <remarks>
/// - <see cref="Width"/> es obligatorio.
/// - Si <see cref="MinWidth"/> no se define, se calcula como Width - 50.
/// - Si <see cref="MaxWidth"/> no se define, se calcula como Width + 50.
/// - Los valores efectivos nunca son negativos.
/// </remarks>
/// <example>
/// <code>
/// [Width(250)]
/// public string Nombre { get; set; }
///
/// [Width(300, MinWidth = 200, MaxWidth = 400)]
/// public string Descripcion { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class WidthAttribute : Attribute
{
    public int MinWidth { get; set; } = -1;
    public int Width { get; set; }
    public int MaxWidth { get; set; } = -1;

    public bool HasMinWidth => MinWidth >= 0;
    public bool HasMaxWidth => MaxWidth >= 0;

    private const int RANGO = 50;

    public int EffectiveMinWidth => HasMinWidth
        ? MinWidth
        : Math.Max(0, Width - RANGO);

    public int EffectiveMaxWidth => HasMaxWidth
        ? MaxWidth
        : Width + RANGO;

    public WidthAttribute(int width)
    {
        Width = width;
    }
}