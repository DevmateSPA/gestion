namespace Gestion.core.attributes;

public class GrupoAttribute : Attribute
{
    public string Nombre { get; set; } = "Sin asignar";
    public int Index { get; set;}

    public GrupoAttribute(string nombre, int index)
    {
        this.Nombre = nombre;
        this.Index = index;
    }

}