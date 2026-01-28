using Gestion.core.model;
using Gestion.core.session;
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
    public LoginWindow(LoginViewModel loginViewModel)
    {
        InitializeComponent();
        _viewModel = loginViewModel;
        DataContext = _viewModel;
    }

    private async void LoginWindow_Loaded(object sender, RoutedEventArgs e)
    {
        txtUsuario.Focus(); 

        await _viewModel.CargarEmpresas();
        cmbEmpresa.SelectedIndex = 0;
    }

    private async void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
    {

        if (_viewModel.EmpresaSeleccionada == null ||
            _viewModel.EmpresaSeleccionada.Id == 0)
        {
            MessageBox.Show("Debe seleccionar una empresa");
            return;
        }

        string nombreUsuario = txtUsuario.Text;
        string clave = txtClave.Password;

        Usuario? usuario = await _viewModel.IniciarSesion(nombreUsuario, clave, _viewModel.EmpresaSeleccionada.Id);
        string mensaje = usuario != null ? "Bienvenido, {usuario.Nombre}!" : "Usuario o clave incorrecta" ;

        if (mensaje.StartsWith("Bienvenido") && usuario != null)
        {
            SesionApp.IdEmpresa = _viewModel.EmpresaSeleccionada.Id;
            SesionApp.NombreEmpresa = _viewModel.EmpresaSeleccionada.Nombre;
            SesionApp.NombreUsuario = usuario.Nombre;
            SesionApp.TipoUsuario = usuario.TipoDesc;
            var main = new MainWindow();
            main.Show();

            this.Close();
        }
        else
            MessageBox.Show(mensaje);
    }
}
