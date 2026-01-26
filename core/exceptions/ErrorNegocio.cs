namespace Gestion.core.exceptions;

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