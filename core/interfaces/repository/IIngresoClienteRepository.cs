using Gestion.core.model;

namespace Gestion.core.interfaces.repository;
public interface IIngresoClienteRepository : IBaseRepository<IngresoCliente>
{
    Task<List<IngresoCliente>> FindAllByEmpresaAndFecha(long empresaId, DateTime desde, DateTime hasta);
    Task<List<IngresoCliente>> FindPageByEmpresaAndFecha(long empresaId, DateTime desde, DateTime hasta, int pageNumber, int pageSize);

}