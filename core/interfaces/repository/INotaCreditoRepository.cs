using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface INotaCreditoRepository : IBaseRepository<NotaCredito>
{
    Task<bool> ExisteFolio(string folio, long empresaId, long? excludeId = null);
}