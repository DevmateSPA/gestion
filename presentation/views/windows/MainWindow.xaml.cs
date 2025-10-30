using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Gestion.presentation.views.pages;

namespace Gestion.presentation.views.windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<LinkItem> Links { get; set; }

        public MainWindow()
        {
            InitializeComponent();

           Links = new List<LinkItem>
            {
                new LinkItem { Nombre = "Bancos", CrearVentana = () => new BancoModalPage() },
                new LinkItem { Nombre = "Clientes", CrearVentana = () => new ClienteModalPage() },
                new LinkItem { Nombre = "Proveedores", CrearVentana = () => new ProveedorModalPage() },
                new LinkItem { Nombre = "Grupos", CrearVentana = () => new GrupoModalPage() },
                new LinkItem { Nombre = "Productos", CrearVentana = () => new ProductoModalPage() },
                new LinkItem { Nombre = "Máquinas", CrearVentana = () => new MaquinaModalPage() },
                new LinkItem { Nombre = "Operarios", CrearVentana = () => new OperarioModalPage() }
            };

            DataContext = this;

            // Navega a la página inicial
            MainFrame.Navigate(new MainPage());
        }

        private void BtnInicio_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MainPage());
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AgregarPage());
        }

        private void Link_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.DataContext is LinkItem link)
            {
                var ventana = link.CrearVentana();
                ventana.Owner = this;
                ventana.ShowDialog();
            }
        }

    }

    public class LinkItem
    {
        public string Nombre { get; set; } 
        public Func<Window> CrearVentana { get; set; } 
    }
}
