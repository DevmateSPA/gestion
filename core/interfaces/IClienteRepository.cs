using Gestion.core.model;

namespace Gestion.core.interfaces;
public interface IClienteRepository
{
    Task<List<Cliente>> GetClientes();
}