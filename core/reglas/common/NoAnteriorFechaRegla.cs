using System.Linq.Expressions;
using Gestion.core.exceptions;
using Gestion.core.interfaces.reglas;

namespace Gestion.core.reglas.common;

public class NoAnteriorFechaRegla<T> : IReglaNegocio<T>
{
    private readonly Func<T, DateTime> _fecha;
    private readonly Func<T, DateTime> _referencia;
    private readonly ErrorNegocio _error;

    public NoAnteriorFechaRegla(
        Expression<Func<T, DateTime>> fecha,
        Expression<Func<T, DateTime>> referencia,
        string mensaje,
        string? campo = null)
    {
        _fecha = fecha.Compile();
        _referencia = referencia.Compile();

        _error = new ErrorNegocio(
            codigo: "DATE_INVALID",
            mensaje: mensaje,
            campo: campo);
    }

    public Task<ErrorNegocio?> Validar(
        T entidad,
        long? _)
    {
        if (_fecha(entidad).Date < _referencia(entidad).Date)
            return Task.FromResult<ErrorNegocio?>(_error);

        return Task.FromResult<ErrorNegocio?>(null);
    }
}