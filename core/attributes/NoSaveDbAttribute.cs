namespace Gestion.core.attributes;

/// <summary>
/// Indica que una propiedad no debe persistirse en la base de datos.
/// </summary>
/// <remarks>
/// - Se utiliza para propiedades calculadas, auxiliares o solo de presentación.
/// - El sistema de persistencia debe ignorar las propiedades marcadas con este atributo.
/// - No afecta la visualización ni la validación en formularios.
/// </remarks>
/// <example>
/// <code>
/// public class OrdenTrabajo
/// {
///     public int Id { get; set; }
///
///     [NoSaveDb]
///     public string EstadoDescripcion => Estado.ToString();
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NoSaveDbAttribute : Attribute
{
    
}
