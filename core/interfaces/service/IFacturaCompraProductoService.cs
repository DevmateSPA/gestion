using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IFacturaCompraProductoService : IBaseService<FacturaCompraProducto>
{
    Task<List<FacturaCompraProducto>> FindByFolio(string folio);
    Task<bool> SaveAll(List<FacturaCompraProducto> detalles);
    Task<bool> UpdateAll(IList<FacturaCompraProducto> detalles);
    Task<bool> DeleteByIds(IList<long> ids);
    Task<bool> DeleteByFolio(string folio);
}