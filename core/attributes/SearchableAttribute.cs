namespace Gestion.core.attributes;

[AttributeUsage(AttributeTargets.Property)]
public class SearchableAttribute : Attribute
{
    public string? SourceKey { get; }
    public string? DisplayMember { get; }

    public SearchableAttribute(
        string? sourceKey = null,
        string? displayMember = null)
    {
        SourceKey = sourceKey;
        DisplayMember = displayMember;
    }
}