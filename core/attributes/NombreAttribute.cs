namespace Gestion.core.attributes;

/// <summary>
/// Define el nombre o texto descriptivo que se mostrará para una propiedad
/// en formularios o vistas generadas dinámicamente.
/// </summary>
/// <remarks>
/// - Se utiliza normalmente para etiquetas (<see cref="Label"/>) o títulos visibles.
/// - Permite desacoplar el nombre mostrado del nombre real de la propiedad.
/// - Si no se define, el sistema puede usar el nombre de la propiedad por defecto.
/// </remarks>
/// <example>
/// <code>
/// [Nombre("Nombre completo")]
/// public string NombreCompleto { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public class NombreAttribute : Attribute
{
    public string Texto { get; }

    public NombreAttribute(string texto)
    {
        Texto = texto;
    }
}