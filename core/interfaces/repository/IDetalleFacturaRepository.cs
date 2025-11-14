using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IDetalleFacturaRepository : IBaseRepository<Detalle>
{
    Task<List<Detalle>> FindByFolio(string folio);
}