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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MenuEntradas.Focus();
        }

        private void MenuItemInfGral_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                string mensaje = item.Tag switch
                {
                    "ModuloBanco" => "Has hecho click en Banco",
                    "ModuloCliente" => "Has hecho click en Cliente",
                    "ModuloProveedor" => "Has hecho click en Proveedor",
                    "ModuloGrupo" => "Has hecho click en Grupo",
                    "ModuloProducto" => "Has hecho click en Producto",
                    "ModuloMaquina" => "Has hecho click en Máquina",
                    "ModuloOperador" => "Has hecho click en Operario",
                    _ => "Módulo desconocido"
                };

                MessageBox.Show(mensaje, "Click en Módulo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuItemCosto_Click(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItemDocVen_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                string mensaje = item.Tag switch
                {
                    "ModuloGuia" => "Has hecho click en Información de Bancos",
                    "ModuloFactura" => "Has hecho click en Información de Clientes",
                    "ModuloNotaCredito" => "Has hecho click en Información de Proveedores",
                    "ModuloDocNulo" => "Has hecho click en Información de Grupos",
                    "ModuloPagoAbono" => "Has hecho click en Información de Productos",
                    "ModuloCancelacion" => "Has hecho click en Información de Maquinas",
                    _ => "Módulo desconocido"
                };

                MessageBox.Show(mensaje, "Documentos de Venta", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuItemDocCom_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                string mensaje = item.Tag switch
                {
                    "ModuloFactura" => "Has hecho click en Facturas",
                    "ModuloNotaCredito" => "Has hecho click en Notas de Crédito",
                    "ModuloPagoAbono" => "Has hecho click en Pagos/Abonos de Facturas",
                    "ModuloCancelacion" => "Has hecho click en Cancelación de Cheques",
                    _ => "Módulo desconocido"
                };

                MessageBox.Show(mensaje, "Documentos de Venta", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void MenuItemControl_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                string mensaje = item.Tag switch
                {
                    "ModuloOrden" => "Has hecho click en Orden de Trabajo",
                    "ModuloOrdenPel" => "Has hecho click en Orden de Trabajo Película",
                    "ModuloSalida" => "Has hecho click en Salida de Productos",
                    "ModuloAjuste" => "Has hecho click en Ajustes de Stock",
                    "ModuloCalculo" => "Has hecho click en Cálculo de Costos",
                    "ModuloCotizacion" => "Has hecho click en Cotizaciones",
                    _ => "Módulo desconocido"
                };

                MessageBox.Show(mensaje, "Control Interno", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();

            login.Show();

            this.Close();
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
