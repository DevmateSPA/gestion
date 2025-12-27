using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IFacturaCompraRepository : IBaseRepository<FacturaCompra>
{
    Task<bool> ExisteFolio(string folio, long empresaId, long? excludeId = null);
}