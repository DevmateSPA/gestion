using Gestion.core.interfaces.reglas;

namespace Gestion.core.reglas.common;

public class UnicoRegla<T>(
    Func<T, long?, Task<bool>> existe,
    Func<T, string> valor,
    string mensaje) : IReglaNegocio<T>
{
    private readonly Func<T, long?, Task<bool>> _existe = existe;
    private readonly Func<T, string> _valor = valor;
    private readonly string _mensaje = mensaje;

    public async Task<string?> Validar(T entidad, long? excludeId)
    {
        bool existe = await _existe(entidad, excludeId);

        return existe
            ? string.Format(_mensaje, _valor(entidad))
            : null;
    }
}