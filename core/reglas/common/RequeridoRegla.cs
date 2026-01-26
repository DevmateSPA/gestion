using System.Linq.Expressions;
using Gestion.core.exceptions;
using Gestion.core.interfaces.reglas;

namespace Gestion.core.reglas.common;

public class RequeridoRegla<T> : IReglaNegocio<T>
{
    private readonly Func<T, object?> _selector;
    private readonly ErrorNegocio _error;

    public RequeridoRegla(
        Expression<Func<T, object?>> selector,
        string mensaje,
        string? campo = null)
    {
        _selector = selector.Compile();
        _error = new ErrorNegocio(
            codigo: "REQUIRED",
            mensaje: mensaje,
            campo: campo);
    }

    public Task<ErrorNegocio?> Validar(
        T entidad,
        long? excludeId = null)
    {
        var valor = _selector(entidad);

        if (valor is null)
            return Task.FromResult<ErrorNegocio?>(_error);

        if (valor is string s && string.IsNullOrWhiteSpace(s))
            return Task.FromResult<ErrorNegocio?>(_error);

        return Task.FromResult<ErrorNegocio?>(null);
    }
}
