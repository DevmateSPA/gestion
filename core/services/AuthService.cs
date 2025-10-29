using Gestion.core.interfaces;
using Gestion.core.model;

namespace Gestion.core.services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AuthService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Usuario?> Login(string nombreUsuario, string contraseña)
    {
        var usuario = await _usuarioRepository.GetByNombre(nombreUsuario);
        
        if (usuario == null || usuario.Contraseña != contraseña)
            return null;
        
        return usuario;
    }
}