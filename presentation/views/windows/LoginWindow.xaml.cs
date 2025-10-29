using Gestion.presentation.viewmodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gestion.presentation.views.windows;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;
    public LoginWindow()
    {
        InitializeComponent();
        _viewModel = new LoginViewModel(App.AuthService);
    }

    private async void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
    {
        string nombreUsuario = txtUsuario.Text;
        string contraseña = txtContraseña.Password;

        string mensaje = await _viewModel.IniciarSesion(nombreUsuario, contraseña);

        if (mensaje.StartsWith("Bienvenido"))
        {
            var main = new MainWindow();
            main.Show();

            this.Close();
        }
        else
            MessageBox.Show(mensaje);
    }
}
