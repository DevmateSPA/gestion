using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IOrdenTrabajoService : IBaseService<OrdenTrabajo>
{
    Task<List<OrdenTrabajo>> FindPageByEmpresaAndPendiente(long empresaId, int pageNumber, int pageSize);
    Task<List<OrdenTrabajo>> FindAllByEmpresaAndPendiente(long empresaId);
    Task<List<OrdenTrabajo>> FindAllByMaquinaWhereEmpresaAndPendiente(long empresaId, string codigoMaquina);
    Task<List<OrdenTrabajo>> FindPageByMaquinaWhereEmpresaAndPendiente( long empresaId, string codigoMaquina, int pageNumber, int pageSize);
    Task<long> ContarPendientes(long empresaId);
    Task<long> ContarByMaquinaWhereEmpresaAndPendientes(long empresaId, string codigoMaquina);
}