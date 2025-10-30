using Gestion.core.model;

namespace Gestion.core.interfaces;
public interface IClienteService
{
    Task<List<Cliente>> GetClientes();
}