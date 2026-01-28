using System.Linq.Expressions;
using Gestion.core.exceptions;
using Gestion.core.interfaces.reglas;

namespace Gestion.core.reglas.common;

public class NoValorPorDefectoRegla<T, TValue> : IReglaNegocio<T>
    where TValue : struct
{
    private readonly Func<T, TValue> _selector;
    private readonly TValue _valorInvalido;
    private readonly ErrorNegocio _error;

    public NoValorPorDefectoRegla(
        Expression<Func<T, TValue>> selector,
        TValue valorInvalido,
        string mensaje,
        string? campo = null)
    {
        _selector = selector.Compile();
        _valorInvalido = valorInvalido;
        _error = new ErrorNegocio(
            codigo: "INVALID_VALUE",
            mensaje: mensaje,
            campo: campo);
    }

    public Task<ErrorNegocio?> Validar(T entidad, long? _)
    {
        var valor = _selector(entidad);

        return EqualityComparer<TValue>.Default.Equals(valor, _valorInvalido)
            ? Task.FromResult<ErrorNegocio?>(_error)
            : Task.FromResult<ErrorNegocio?>(null);
    }
}
