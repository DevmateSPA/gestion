using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IFotomecanicaRepository : IBaseRepository<Fotomecanica>
{
    Task<bool> ExisteCodigo(string codigo, long empresaId);
}