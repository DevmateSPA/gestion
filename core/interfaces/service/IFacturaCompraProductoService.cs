using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IFacturaCompraProductoService : IBaseService<FacturaCompraProducto>
{
    Task<List<FacturaCompraProducto>> FindByFolio(string folio);
}