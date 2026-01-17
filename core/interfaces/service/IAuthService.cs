using Gestion.core.model;

namespace Gestion.core.interfaces.service;

public interface IAuthService
{
    Task<Usuario?> Login(string nombreUsuario, string clave);
}