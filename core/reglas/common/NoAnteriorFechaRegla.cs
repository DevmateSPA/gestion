using System.Linq.Expressions;
using Gestion.core.interfaces.reglas;

namespace Gestion.core.reglas.common;

public class NoAnteriorFechaRegla<T>(
    Expression<Func<T, DateTime>> fecha,
    Expression<Func<T, DateTime>> referencia,
    string mensaje) : IReglaNegocio<T>
{
    private readonly Func<T, DateTime> _fecha = fecha.Compile();
    private readonly Func<T, DateTime> _referencia = referencia.Compile();
    private readonly string _mensaje = mensaje;

    public Task<string?> Validar(T entidad, long? _)
    {
        return Task.FromResult(
            _fecha(entidad).Date < _referencia(entidad).Date
                ? _mensaje
                : null
        );
    }
}