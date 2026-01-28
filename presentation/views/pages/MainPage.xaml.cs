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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gestion.core.session;

namespace Gestion.presentation.views.pages
{
    /// <summary>
    /// Lógica de interacción para MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            txtEmpresa.Text = $"Empresa: {SesionApp.NombreEmpresa}";
            txtUsuario.Text = $"Usuario: {SesionApp.NombreUsuario}";
            txtFecha.Text = $"Fecha: {DateTime.Now.ToString("dd/MM/yyyy")}";

            CargarLogo();
        }

        private void CargarLogo()
        {
            string logo = SesionApp.IdEmpresa switch
            {
                1 => "logo1.png",
                2 => "logo2.png",
                _ => "logo.png"
            };

            imgLogo.Source = new BitmapImage(
                new Uri($"/resources/{logo}", UriKind.Relative)
            );
        }
    }
}
