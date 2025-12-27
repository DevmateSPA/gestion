using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IBancoRepository : IBaseRepository<Banco>
{
    Task<bool> ExisteCodigo(string codigo, long empresaId, long? excludeId = null);
}