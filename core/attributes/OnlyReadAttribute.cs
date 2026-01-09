namespace  Gestion.core.attributes;

/// <summary>
/// Indica que una propiedad debe mostrarse en modo solo lectura
/// en formularios o vistas generadas dinámicamente.
/// </summary>
/// <remarks>
/// - El control asociado se renderiza como solo lectura o deshabilitado,
///   según el tipo de control.
/// - No permite edición por parte del usuario.
/// - No afecta la persistencia en base de datos.
/// </remarks>
/// <example>
/// <code>
/// public class Usuario
/// {
///     public string Nombre { get; set; }
///
///     [OnlyRead]
///     public string Rol { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class OnlyReadAttribute : Attribute
{
    
}