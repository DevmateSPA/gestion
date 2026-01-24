using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IFacturaCompraProductoService : IBaseService<FacturaCompraProducto>
{
    Task<List<FacturaCompraProducto>> FindByFolio(string folio, long empresaId);
    Task<bool> SaveAll(List<FacturaCompraProducto> detalles);
    Task<bool> UpdateAll(IList<FacturaCompraProducto> detalles, long empresaId);
    Task<bool> DeleteByIds(IList<long> ids, long empresaId);
    Task<bool> DeleteByFolio(string folio, long empresaId);
}