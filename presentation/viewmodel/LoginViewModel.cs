using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class LoginViewModel
{
    private readonly IAuthService _authService;
    private readonly IEmpresaService _empresaService;

    public LoginViewModel(IAuthService authService, IEmpresaService empresaService)
    {
        _authService = authService;
        _empresaService = empresaService;
    }

    public async Task<List<Empresa>> CargarEmpresas()
    {
        var empresas = await _empresaService.FindAll();

        // Agregar opción por defecto
        empresas.Insert(0, new Empresa
        {
            Id = 0,
            Nombre = "Seleccione empresa"
        });

        return empresas;
    }

    public async Task<string> IniciarSesion(string nombreUsuario, string contraseña)
    {
        var usuario = await _authService.Login(nombreUsuario, contraseña);

        if (usuario == null)
            return "Usuario o contraseña incorrecta";

        return $"Bienvenido, {usuario.NombreUsuario}!";
    }
}