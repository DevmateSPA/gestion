using Gestion.core.model;

namespace Gestion.core.interfaces;

public interface IAuthService
{
    Task<Usuario?> LoginAsync(string nombreUsuario, string contraseña);
}