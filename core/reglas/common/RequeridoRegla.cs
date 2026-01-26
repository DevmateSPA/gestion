using System.Linq.Expressions;
using Gestion.core.interfaces.reglas;

namespace Gestion.core.reglas.common;

public class RequeridoRegla<T>(
    Expression<Func<T, string?>> selector,
    string mensaje) : IReglaNegocio<T>
{
    private readonly Func<T, string?> _selector = selector.Compile();
    private readonly string _mensaje = mensaje;

    public Task<string?> Validar(T entidad, long? _)
    {
        var valor = _selector(entidad);

        return Task.FromResult(
            string.IsNullOrWhiteSpace(valor)
                ? _mensaje
                : null
        );
    }
}