using System.Text;

namespace Gestion.core.exceptions;

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