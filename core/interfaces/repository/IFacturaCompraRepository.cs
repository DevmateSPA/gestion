using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IFacturaCompraRepository : IBaseRepository<FacturaCompra>
{
    Task<List<FacturaCompra>> FindAllWithDetails();
    
}