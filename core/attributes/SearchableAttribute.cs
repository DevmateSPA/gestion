namespace Gestion.core.attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SearchableAttribute : Attribute
{
    public string? SourceKey { get; }

    public SearchableAttribute(
        string? sourceKey = null)
    {
        SourceKey = sourceKey;
    }
}