namespace Gestion.core.attributes;

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