using Gestion.core.exceptions;
using Gestion.core.interfaces.reglas;

namespace Gestion.core.reglas.common;

public class UnicoRegla<T> : IReglaNegocio<T>
{
    private readonly Func<T, long?, Task<bool>> _existe;
    private readonly Func<T, object?> _valor;
    private readonly string _mensaje;
    private readonly string? _campo;

    public UnicoRegla(
        Func<T, long?, Task<bool>> existe,
        Func<T, object?> valor,
        string mensaje,
        string? campo = null)
    {
        _existe = existe;
        _valor = valor;
        _mensaje = mensaje;
        _campo = campo;
    }

    public async Task<ErrorNegocio?> Validar(
        T entidad,
        long? excludeId = null)
    {
        if (!await _existe(entidad, excludeId))
            return null;

        var valor = _valor(entidad);

        return new ErrorNegocio(
            codigo: "UNIQUE",
            mensaje: string.Format(_mensaje, valor),
            campo: _campo);
    }
}