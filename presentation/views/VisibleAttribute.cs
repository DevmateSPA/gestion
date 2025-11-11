using System;

[AttributeUsage(AttributeTargets.Property)]
public class VisibleAttribute : Attribute
{
    public bool Mostrar { get; }

    public VisibleAttribute(bool mostrar = true)
    {
        Mostrar = mostrar;
    }
}
