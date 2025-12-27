using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IGrupoRepository : IBaseRepository<Grupo>
{
    Task<bool> ExisteCodigo(string codigo, long empresaId, long? excludeId = null);
}