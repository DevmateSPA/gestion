using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IVentaRepository : IBaseRepository<Venta>
{
    Task<bool> ExisteFolio(string folio, long empresaId, long? excludeId = null);
}