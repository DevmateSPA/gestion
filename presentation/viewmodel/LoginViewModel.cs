using Gestion.core.interfaces.service;

namespace Gestion.presentation.viewmodel;

public class LoginViewModel
{
    private readonly IAuthService _authService;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    public async Task<string> IniciarSesion(string nombreUsuario, string contraseña)
    {
        var usuario = await _authService.Login(nombreUsuario, contraseña);

        if (usuario == null)
            return "Usuario o contraseña incorrecta";

        return $"Bienvenido, {usuario.NombreUsuario}!";
    }
}