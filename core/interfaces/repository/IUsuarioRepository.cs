using Gestion.core.model;
using Gestion.core.model.DTO;

namespace Gestion.core.interfaces.repository;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario?> GetByNombre(string nombre, long empresaId);
    Task<List<TipoUsuarioDTO>> GetTipoList();
}