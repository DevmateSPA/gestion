using System.Text;

namespace Gestion.core.exceptions;

/// <summary>
/// Excepción lanzada cuando una o más reglas de negocio
/// del dominio no se cumplen.
/// </summary>
/// <remarks>
/// Esta excepción agrupa uno o más <see cref="ErrorNegocio"/> producidos
/// durante la validación de una entidad.
/// 
/// No representa errores técnicos ni fallos del sistema,
/// sino condiciones válidas del dominio que impiden completar
/// una operación (por ejemplo, violaciones de unicidad,
/// estados inválidos, etc.).
/// </remarks>
public sealed class ReglaNegocioException : Exception
{
    public IReadOnlyCollection<ErrorNegocio> Errores { get; }

    public ReglaNegocioException(
        IReadOnlyCollection<ErrorNegocio> errores)
        : base(ConstruirMensaje(errores))
    {
        Errores = errores;
    }

    private static string ConstruirMensaje(
        IReadOnlyCollection<ErrorNegocio> errores)
    {
        if (errores.Count == 0)
            return "Se incumplieron reglas de negocio.";

        var sb = new StringBuilder();
        sb.AppendLine("Se incumplieron las siguientes reglas de negocio:");

        foreach (var error in errores)
        {
            if (!string.IsNullOrWhiteSpace(error.Campo))
                sb.Append($"- [{error.Campo}] ");

            sb.AppendLine(error.Mensaje);
        }

        return sb.ToString();
    }
}