namespace Gestion.core.attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RadioGroupAttribute : Attribute
{
    public (string Texto, string Valor)[] Opciones { get; }

    public RadioGroupAttribute(params object[] opciones)
    {
        // Las opciones se pasan como pares Texto, Valor
        var lista = new List<(string, string)>();
        for (int i = 0; i < opciones.Length; i += 2)
        {
            lista.Add((opciones[i].ToString()!, opciones[i + 1].ToString()!));
        }
        Opciones = [.. lista];
    }
}