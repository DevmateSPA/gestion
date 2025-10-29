using Gestion.core.model;

namespace Gestion.core.interfaces;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByNombre(string nombre);
}