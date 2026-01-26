namespace Gestion.core.interfaces.reglas;

public interface IReglaNegocio<T>
{
    Task<string?> Validar(T entidad, long? excludeId = null);
}