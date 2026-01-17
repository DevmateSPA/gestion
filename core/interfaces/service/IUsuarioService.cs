using Gestion.core.model;
using Gestion.core.model.DTO;

namespace Gestion.core.interfaces.service;
public interface IUsuarioService : IBaseService<Usuario>
{
    Task<Usuario?> GetByNombre(string nombreUsuario, long empresaId);
    Task<List<TipoUsuarioDTO>> GetTipoList();
}