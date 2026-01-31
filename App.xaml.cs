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
using Gestion.presentation.views.util;
using Gestion.presentation.config;
using Serilog;
using Gestion.core.interfaces.lookup;
using Gestion.core.lookup;

namespace Gestion;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Debug()
        .WriteTo.File(
            "logs/app-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7)
        .CreateLogger();

        UiConfig.Register(); // Llama los registros de los dg personalziados

        DispatcherUnhandledException += (s, ex) =>
        {
            Log.Fatal(ex.Exception, "Excepción no controlada en UI Thread");

            var dialog = ServiceProvider?
                .GetService<IDialogService>();

            if (dialog != null)
            {
                dialog.ShowError(
                    ex.Exception.Message,
                    "Error inesperado");
            }
            else
            {
                MessageBox.Show(
                    ex.Exception.Message,
                    "Error inesperado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            ex.Handled = true; // evita que WPF cierre la app
        };

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
        services.AddScoped<IVentaRepository, VentaRepository>();
        services.AddScoped<IFacturaRepository, FacturaRepository>();
        services.AddScoped<IFacturaCompraRepository, FacturaCompraRepository>();
        services.AddScoped<IFacturaCompraProductoRepository, FacturaCompraProductoRepository>();
        services.AddScoped<IGuiaDespachoRepository, GuiaDespachoRepository>();
        services.AddScoped<IOrdenTrabajoRepository, OrdenTrabajoRepository>();
        services.AddScoped<IEmpresaRepository, EmpresaRepository>();
        services.AddScoped<IDetalleOTRepository, DetalleOTRepository>();
        services.AddScoped<IIngresoClienteRepository, IngresoClienteRepository>();

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
        services.AddScoped<IVentaService, VentaService>();
        services.AddScoped<IFacturaService, FacturaService>();
        services.AddScoped<IFacturaCompraService, FacturaCompraService>();
        services.AddScoped<IFacturaCompraProductoService, FacturaCompraProductoService>();
        services.AddScoped<IGuiaDespachoService, GuiaDespachoService>();
        services.AddScoped<IOrdenTrabajoService, OrdenTrabajoService>();
        services.AddScoped<IEmpresaService, EmpresaService>();
        services.AddScoped<IDetalleOTService, DetalleOTService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IIngresoClienteService, IngresoClienteService>();

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
        // GuiaDespacho
        services.AddTransient<GuiaDespachoViewModel>();
        services.AddTransient<GuiaDespachoPage>();
        // Factura
        services.AddTransient<FacturaViewModel>();
        services.AddTransient<FacturaPage>();
        // NotaCredito
        services.AddTransient<NotaCreditoViewModel>();
        services.AddTransient<NotaCreditoPage>();
        // DocumentoNulo
        services.AddTransient<DocumentoNuloViewModel>();
        services.AddTransient<DocumentoNuloPage>();
        // Venta
        services.AddTransient<VentaViewModel>();
        services.AddTransient<VentaPage>();
        // FacturaCompra
        services.AddTransient<FacturaCompraViewModel>();
        services.AddTransient<FacturaCompraPage>();        
        // OrdenTrabajo
        services.AddTransient<OrdenTrabajoViewModel>();
        services.AddTransient<OrdenTrabajoPage>();
        // OrdenTrabajoPrueba
        services.AddTransient<OrdenTrabajoDetallePage>();
        // CuentaCorrienteCliente
        services.AddTransient<CuentaCorrienteClientePage>();   
        // IngresosCliente
        services.AddTransient<IngresoClienteViewModel>();
        services.AddTransient<IngresoClientePage>();   
        // PendientesProduccion
        services.AddTransient<PendienteProduccionPage>();   
        // Maquinas Con OT pendiente
        services.AddTransient<MaquinasConOTPendientesPage>();
        // Usuarios
        services.AddTransient<UsuarioViewModel>();
        services.AddTransient<UsuariosPage>();
        // Ventanas modales
        services.AddTransient<AgregarBancoWindow>();
        services.AddTransient<ClienteCartolaWindow>();
        services.AddTransient<ClienteOTWindow>();
        services.AddTransient<PendienteMaquinaWindow>();      

        services.AddSingleton<ILookupResolver, LookupResolver>();
       
        ServiceProvider = services.BuildServiceProvider();

        DialogUtils.Init(ServiceProvider.GetRequiredService<IDialogService>());

        var login = ServiceProvider.GetRequiredService<LoginWindow>();
        login.Show();
    }

    // Libera al cerrar
    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
