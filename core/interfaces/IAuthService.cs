using Gestion.core.model;

namespace Gestion.core.interfaces;

public interface IAuthService
{
    Task<Usuario?> Login(string nombreUsuario, string contraseña);
}