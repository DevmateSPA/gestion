using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario?> GetByNombre(string nombre);
}