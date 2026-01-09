namespace Gestion.core.attributes;

/// <summary>
/// Define el grupo al que pertenece una propiedad dentro de un formulario
/// o vista generada dinámicamente.
/// </summary>
/// <remarks>
/// - <see cref="Nombre"/> indica el nombre del grupo (ej. pestaña, sección o bloque).
/// - <see cref="Index"/> determina el orden de aparición del grupo.
/// - Las propiedades con el mismo nombre de grupo se renderizan juntas.
/// </remarks>
/// <example>
/// <code>
/// [Grupo("Datos generales", 0)]
/// public string Nombre { get; set; }
///
/// [Grupo("Datos generales", 0)]
/// public string Rut { get; set; }
///
/// [Grupo("Configuración", 1)]
/// public bool Activo { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GrupoAttribute : Attribute
{
    public string Nombre { get; set; } = "Sin asignar";
    public int Index { get; set;}

    public GrupoAttribute(string nombre, int index)
    {
        this.Nombre = nombre;
        this.Index = index;
    }

}