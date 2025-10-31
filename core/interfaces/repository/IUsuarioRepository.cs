using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByNombre(string nombre);
}