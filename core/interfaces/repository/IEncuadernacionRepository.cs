using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IEncuadernacionRepository : IBaseRepository<Encuadernacion>
{
    Task<bool> ExisteCodigo(string codigo, long empresaId);
}