using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IFacturaCompraProductoRepository : IBaseRepository<FacturaCompraProducto>
{
    Task<List<FacturaCompraProducto>> FindByFolio(string folio);
    Task<bool> SaveAll(IList<FacturaCompraProducto> detalles);
    Task<bool> UpdateAll(IList<FacturaCompraProducto> detalles);
    Task<bool> DeleteByIds(IList<long> ids);
    Task<bool> DeleteByFolio(string folio);
}