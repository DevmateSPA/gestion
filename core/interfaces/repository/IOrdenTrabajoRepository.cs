using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IOrdenTrabajoRepository : IBaseRepository<OrdenTrabajo>
{
    Task<List<OrdenTrabajo>> FindAllByEmpresa(long empresaId);
}