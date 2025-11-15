using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IFacturaRepository : IBaseRepository<Factura>
{
    Task<List<Factura>> FindAllWithDetails();
}