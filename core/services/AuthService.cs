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

    public async Task<Usuario?> Login(
        string nombreUsuario, 
        string clave, 
        long empresaId)
    {
        var usuario = await _usuarioRepository.GetByNombre(nombreUsuario, empresaId);
        
        if (usuario == null || !usuario.Clave.Equals(clave, StringComparison.CurrentCultureIgnoreCase))
            return null;
        
        return usuario;
    }
}