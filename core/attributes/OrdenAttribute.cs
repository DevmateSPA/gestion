namespace Gestion.core.attributes;

/// <summary>
/// Define el orden en que una propiedad debe ser presentada
/// en interfaces de usuario o procesos de renderizado dinámico.
/// </summary>
/// <remarks>
/// - Se usa normalmente para ordenar campos dentro de un grupo o formulario.
/// - Un valor menor indica mayor prioridad.
/// - Si no se especifica, el sistema puede aplicar un orden por defecto.
/// </remarks>
/// <example>
/// <code>
/// [Orden(1)]
/// public string Rut { get; set; }
///
/// [Orden(2)]
/// public string Nombre { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public class OrdenAttribute : Attribute
{
    public int Index { get; }

    public OrdenAttribute(int index)
    {
        Index = index;
    }
}