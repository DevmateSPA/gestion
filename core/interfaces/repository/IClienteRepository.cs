using Gestion.core.model;

namespace Gestion.core.interfaces.repository;
public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<List<string>> GetRutList(string busquedaRut, long empresaId);
}