using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IOrdenTrabajoRepository : IBaseRepository<OrdenTrabajo>
{
    Task<List<OrdenTrabajo>> FindPageByEmpresaAndPendiente(long empresaId, int pageNumber, int pageSize);
}