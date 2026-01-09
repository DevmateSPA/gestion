namespace Gestion.core.attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class WidthAttribute : Attribute
{
    public int MinWidth { get; set; } = -1;
    public int Width { get; set; }
    public int MaxWidth { get; set; } = -1;

    public bool HasMinWidth => MinWidth >= 0;
    public bool HasMaxWidth => MaxWidth >= 0;

    public WidthAttribute(int width)
    {
        Width = width;
    }
}