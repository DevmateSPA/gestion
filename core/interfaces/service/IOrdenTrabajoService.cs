using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IOrdenTrabajoService : IBaseService<OrdenTrabajo>
{
    Task<List<OrdenTrabajo>> FindPageByEmpresaAndPendiente(long empresaId, int pageNumber, int pageSize);
}