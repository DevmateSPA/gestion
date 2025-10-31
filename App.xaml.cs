using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.services;
using Gestion.Infrastructure.data;
using Gestion.Infrastructure.Services;
using Gestion.presentation.views.windows;

namespace Gestion;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IAuthService AuthService { get; private set; } = null!;
    public static IClienteService ClienteService { get; private set; } = null!;
    public static IProveedorService ProveedorService { get; private set; } = null!;
    public static IBancoService BancoService { get; private set; } = null!;
    public static IGrupoService GrupoService { get; private set; } = null!;
    public static IProductoService ProductoService { get; private set; } = null!;
    public static IMaquinaService MaquinaService { get; private set; } = null!;
    public static IOperadorService OperadorService { get; private set; } = null!;
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var connectionFactory = new MySqlConnectionFactory();
        var usuarioRepository = new UsuarioRepository(connectionFactory);
        var clienteRepository = new ClienteRepository(connectionFactory);
        var proveedorRepository = new ProveedorRepository(connectionFactory);
        var bancoRepository = new BancoRepository(connectionFactory);
        var grupoRepository = new GrupoRepository(connectionFactory);
        var productoRepository = new ProductoRepository(connectionFactory);
        var maquinaRepository = new MaquinaRepository(connectionFactory);
        var operadorRepository = new OperadorRepository(connectionFactory);
        App.AuthService = new AuthService(usuarioRepository);
        App.ClienteService = new ClienteService(clienteRepository);
        App.ProveedorService = new ProveedorService(proveedorRepository);
        App.BancoService = new BancoService(bancoRepository);
        App.GrupoService = new GrupoService(grupoRepository);
        App.ProductoService = new ProductoService(productoRepository);
        App.MaquinaService = new MaquinaService(maquinaRepository);
        App.OperadorService = new OperadorService(operadorRepository);

        LoginWindow login = new LoginWindow();
        login.Show();
    }
}

