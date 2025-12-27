using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IImpresionRepository : IBaseRepository<Impresion>
{
    Task<bool> ExisteCodigo(string codigo, long empresaId, long? excludeId = null);
}