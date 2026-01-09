namespace Gestion.core.attributes;

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