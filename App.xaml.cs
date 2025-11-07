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
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gestion;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();
        services.AddSingleton<IDialogService, DialogService>();

        // Repositorios
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IProveedorRepository, ProveedorRepository>();
        services.AddScoped<IBancoRepository, BancoRepository>();
        services.AddScoped<IGrupoRepository, GrupoRepository>();
        services.AddScoped<IProductoRepository, ProductoRepository>();
        services.AddScoped<IMaquinaRepository, MaquinaRepository>();
        services.AddScoped<IOperarioRepository, OperarioRepository>();
        services.AddScoped<IImpresionRepository, ImpresionRepository>();
        services.AddScoped<IEncuadernacionRepository, EncuadernacionRepository>();
        services.AddScoped<IFotomecanicaRepository, FotomecanicaRepository>();
        services.AddScoped<INotaCreditoRepository, NotaCreditoRepository>();
        services.AddScoped<IDocumentoNuloRepository, DocumentoNuloRepository>();
        services.AddScoped<IFacturaRepository, FacturaRepository>();
        services.AddScoped<IFacturaCompraRepository, FacturaCompraRepository>();

        // Servicios
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IProveedorService, ProveedorService>();
        services.AddScoped<IBancoService, BancoService>();
        services.AddScoped<IGrupoService, GrupoService>();
        services.AddScoped<IProductoService, ProductoService>();
        services.AddScoped<IMaquinaService, MaquinaService>();
        services.AddScoped<IOperarioService, OperarioService>();
        services.AddScoped<IImpresionService, ImpresionService>();
        services.AddScoped<IEncuadernacionService, EncuadernacionService>();
        services.AddScoped<IFotomecanicaService, FotomecanicaService>();
        services.AddScoped<INotaCreditoService, NotaCreditoService>();
        services.AddScoped<IDocumentoNuloService, DocumentoNuloService>();
        services.AddScoped<IFacturaService, FacturaService>();
        services.AddScoped<IFacturaCompraService, FacturaCompraService>();

        // Login
        services.AddTransient<LoginWindow>();
        services.AddTransient<LoginViewModel>();

        // Banco
        services.AddTransient<BancoViewModel>();
        services.AddTransient<BancoPage>();
        //Cliente
        services.AddTransient<ClienteViewModel>();
        services.AddTransient<ClientePage>();
        // Grupo
        services.AddTransient<GrupoViewModel>();
        services.AddTransient<GrupoPage>();
        // Maquinas
        services.AddTransient<MaquinaViewModel>();
        services.AddTransient<MaquinaPage>();
        // Operario
        services.AddTransient<OperarioViewModel>();
        services.AddTransient<OperarioPage>();
        // Producto
        services.AddTransient<ProductoViewModel>();
        services.AddTransient<ProductoPage>();
        // Proveedor
        services.AddTransient<ProveedorViewModel>();
        services.AddTransient<ProveedorPage>();
        // Impresion
        services.AddTransient<ImpresionViewModel>();
        services.AddTransient<ImpresionPage>();
        // Encuadernacion
        services.AddTransient<EncuadernacionViewModel>();
        services.AddTransient<EncuadernacionPage>();
        // Fotomecanica
        services.AddTransient<FotomecanicaViewModel>();
        services.AddTransient<FotomecanicaPage>();

        // Ventanas modales
        services.AddTransient<AgregarBancoWindow>();
       
        ServiceProvider = services.BuildServiceProvider();

        var login = ServiceProvider.GetRequiredService<LoginWindow>();
        login.Show();
    }
}
