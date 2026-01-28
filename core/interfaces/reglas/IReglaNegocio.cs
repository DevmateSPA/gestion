using Gestion.core.exceptions;

namespace Gestion.core.interfaces.reglas;

public interface IReglaNegocio<T>
{
    Task<ErrorNegocio?> Validar(
        T entidad, 
        long? excludeId = null);
}