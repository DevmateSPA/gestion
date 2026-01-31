using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IClienteService : IBaseService<Cliente>
{
    Task<List<string>> GetRutList(string busquedaRut, long empresaId);
    Task<Cliente?> FindByRut(string rut, long empresaId);
}