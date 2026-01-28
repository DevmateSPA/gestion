using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface INotaCreditoRepository : IBaseRepository<NotaCredito>
{
    Task<string> GetSiguienteFolio(long empresaId);
}