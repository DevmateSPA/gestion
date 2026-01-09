namespace Gestion.core.attributes;

/// <summary>
/// Indica que un método es seguro para ejecutarse desde la capa de interfaz de usuario.
/// </summary>
/// <remarks>
/// - Señala que el método no realiza operaciones bloqueantes.
/// - No ejecuta acceso directo a la base de datos ni llamadas remotas.
/// - No produce efectos secundarios peligrosos para la UI.
/// - Puede ejecutarse sin riesgo desde el hilo de interfaz.
/// </remarks>
/// <example>
/// <code>
/// [UiSafe]
/// public void ActualizarVista()
/// {
///     // Lógica liviana de UI
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Method)]
public sealed class UiSafeAttribute : Attribute
{
}