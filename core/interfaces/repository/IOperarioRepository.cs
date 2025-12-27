using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IOperarioRepository : IBaseRepository<Operario>
{
    Task<bool> ExisteCodigo(string codigo, long empresaId, long? excludeId = null);
}