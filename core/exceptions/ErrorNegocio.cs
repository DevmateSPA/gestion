namespace Gestion.core.exceptions;

/// <summary>
/// Representa un error de negocio producido por la violación
/// de una o más reglas del dominio.
/// </summary>
/// <remarks>
/// Esta clase se utiliza como resultado de validaciones de reglas de negocio
/// y es transportada por <see cref="ReglaNegocioException"/> para comunicar
/// errores de forma estructurada.
/// 
/// No representa errores técnicos ni excepciones del sistema,
/// sino condiciones válidas del dominio que impiden continuar la operación.
/// </remarks>
public sealed class ErrorNegocio(
    string codigo,
    string mensaje,
    string? campo = null)
{
    public string Codigo { get; } = codigo;
    public string Mensaje { get; } = mensaje;
    public string? Campo { get; } = campo;

    public override string ToString() => Mensaje;
}