[AttributeUsage(AttributeTargets.Property)]
public class NombreAttribute : Attribute
{
    public string Texto { get; }

    public NombreAttribute(string texto)
    {
        Texto = texto;
    }
}