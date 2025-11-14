[AttributeUsage(AttributeTargets.Property)]
public class OrdenAttribute : Attribute
{
    public int Index { get; }

    public OrdenAttribute(int index)
    {
        Index = index;
    }
}