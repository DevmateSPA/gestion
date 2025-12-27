using Gestion.core.model;

namespace Gestion.core.interfaces.repository;
public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<bool> ExisteRut(string rut, long empresaId, long? excludeId = null);
}