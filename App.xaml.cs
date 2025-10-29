using System.Configuration;
using System.Data;
using System.Windows;
using Gestion.core.interfaces;
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
    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var connectionFactory = new MySqlConnectionFactory();
        var usuarioRepository = new UsuarioRepository(connectionFactory);
        var clienteRepository = new ClienteRepository(connectionFactory);
        App.AuthService = new AuthService(usuarioRepository);
        App.ClienteService = new ClienteService(clienteRepository);

        LoginWindow login = new LoginWindow();
        login.Show();
    }
}

