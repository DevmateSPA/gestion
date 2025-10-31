using Gestion.core.model;

namespace Gestion.core.interfaces;
public interface IClienteRepository : IBaseRepository<Cliente>
{
    Task<List<Cliente>> GetClientes();
}