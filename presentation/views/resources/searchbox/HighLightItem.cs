namespace Gestion.presentation.views.resources.searchbox;

public class HighlightItem
{
    public string Original { get; }

    public string Prefix { get; }
    public string Match { get; }
    public string Suffix { get; }

    public HighlightItem(string text, string query)
    {
        Original = text;

        if (string.IsNullOrWhiteSpace(query))
        {
            Prefix = text;
            Match = "";
            Suffix = "";
            return;
        }

        var index = text.IndexOf(query, StringComparison.OrdinalIgnoreCase);

        if (index < 0)
        {
            Prefix = text;
            Match = "";
            Suffix = "";
            return;
        }

        Prefix = text.Substring(0, index);
        Match  = text.Substring(index, query.Length);
        Suffix = text.Substring(index + query.Length);
    }

    public override string ToString() => Original;
}