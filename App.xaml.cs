using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Gestion.core.interfaces.service;
using Gestion.core.services;
using Gestion.Infrastructure.data;
using Gestion.Infrastructure.Services;
using Gestion.presentation.views.windows;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.pages;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.database;

namespace Gestion;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();

        // Repositorios
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<IBancoRepository, BancoRepository>();
        services.AddScoped<IGrupoRepository, GrupoRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IMaquinaRepository, MaquinaRepository>();
        services.AddScoped<IOperadorRepository, OperadorRepository>();

        // Servicios
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IProveedorService, ProveedorService>();
        services.AddScoped<IBancoService, BancoService>();
        services.AddScoped<IGrupoService, GrupoService>();
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<IMaquinaService, MaquinaService>();
        services.AddScoped<IOperadorService, OperadorService>();

        // Login
        services.AddTransient<LoginWindow>();
        services.AddTransient<LoginViewModel>();

        // Agregar
        services.AddTransient<AgregarViewModel>();
        services.AddTransient<AgregarPage>();

        // Banco
        services.AddTransient<BancoViewModel>();
        services.AddTransient<BancoModalPage>();
        //Cliente
        services.AddTransient<ClienteViewModel>();
        services.AddTransient<ClienteModalPage>();
        // Grupo
        services.AddTransient<GrupoViewModel>();
        services.AddTransient<GrupoModalPage>();
        // Maquinas
        services.AddTransient<MaquinaViewModel>();
        services.AddTransient<MaquinaModalPage>();
        // Operario
        services.AddTransient<OperarioViewModel>();
        services.AddTransient<OperarioModalPage>();
        // Producto
        services.AddTransient<ProductoViewModel>();
        services.AddTransient<ProductoModalPage>();
        // Proveedor
        services.AddTransient<ProveedorViewModel>();
        services.AddTransient<ProveedorModalPage>();

        ServiceProvider = services.BuildServiceProvider();

        var login = ServiceProvider.GetRequiredService<LoginWindow>();
        login.Show();
    }
}
