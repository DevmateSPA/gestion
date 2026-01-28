using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IIngresoClienteService : IBaseService<IngresoCliente>
{

    Task<List<IngresoCliente>> LoadAllByEmpresaAndFecha(long empresaId, DateTime desde, DateTime hasta);
    Task<List<IngresoCliente>> LoadPageByEmpresaAndFecha(long empresaId, DateTime desde, DateTime hasta, int pageNumber, int pageSize);
}