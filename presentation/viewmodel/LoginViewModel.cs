using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class LoginViewModel
{
    private readonly IAuthService _authService;
    private readonly IEmpresaService _empresaService;
    protected readonly IDialogService _dialogService;
    public ObservableCollection<Empresa> Empresas { get; } = new();
    public Empresa? EmpresaSeleccionada { get; set; }

    public LoginViewModel(IAuthService authService, IEmpresaService empresaService, IDialogService dialogService)
    {
        _authService = authService;
        _empresaService = empresaService;
        _dialogService = dialogService;
    }

    public async Task CargarEmpresas()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var empresas = await _empresaService.FindAll();

            if (empresas == null || !empresas.Any())
                throw new Exception("No hay empresas cargadas");

            Empresas.Clear();

            // Opción por defecto
            Empresas.Add(new Empresa
            {
                Id = 0,
                Nombre = "Seleccione empresa"
            });

            foreach (var emp in empresas)
                Empresas.Add(emp);

            EmpresaSeleccionada = Empresas.FirstOrDefault();

        }, _dialogService, "Error al cargar las empresas");
    }

    public async Task<string> IniciarSesion(string nombreUsuario, string contraseña)
    {
        var usuario = await _authService.Login(nombreUsuario, contraseña);

        if (usuario == null)
            return "Usuario o contraseña incorrecta";

        return $"Bienvenido, {usuario.NombreUsuario}!";
    }
}