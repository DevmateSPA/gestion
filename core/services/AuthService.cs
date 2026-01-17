using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public AuthService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<Usuario?> Login(string nombreUsuario, string clave)
    {
        var usuario = await _usuarioRepository.GetByNombre(nombreUsuario);
        
        if (usuario == null || usuario.Clave.ToLower() != clave.ToLower())
            return null;
        
        return usuario;
    }
}