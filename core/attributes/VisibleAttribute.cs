namespace Gestion.core.attributes;

/// <summary>
/// Indica si una propiedad debe ser visible en la interfaz de usuario.
/// </summary>
/// <remarks>
/// - Por defecto, la propiedad es visible.
/// - Puede usarse para ocultar campos técnicos o internos.
/// - No afecta la persistencia ni la validación, solo la presentación.
/// </remarks>
/// <example>
/// <code>
/// [Visible]
/// public string Nombre { get; set; }
///
/// [Visible(false)]
/// public int IdInterno { get; set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property)]
public sealed class VisibleAttribute : Attribute
{
    /// <summary>
    /// Indica si la propiedad debe mostrarse en la UI.
    /// </summary>
    public bool Mostrar { get; }

    /// <summary>
    /// Crea el atributo indicando si la propiedad es visible.
    /// </summary>
    /// <param name="mostrar">
    /// true para mostrar la propiedad (valor por defecto),
    /// false para ocultarla.
    /// </param>
    public VisibleAttribute(bool mostrar = true)
    {
        Mostrar = mostrar;
    }
}
