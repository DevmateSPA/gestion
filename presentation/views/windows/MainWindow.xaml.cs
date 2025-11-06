using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.presentation.views.pages;
using Microsoft.Extensions.DependencyInjection;

namespace Gestion.presentation.views.windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

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
            try
            {
                if (sender is MenuItem item)
                {
                    Page? page = item.Tag switch
                    {
                        "ModuloBanco" => App.ServiceProvider.GetRequiredService<BancoPage>(),
                        "ModuloCliente" => App.ServiceProvider.GetRequiredService<ClientePage>(),
                        "ModuloProveedor" => App.ServiceProvider.GetRequiredService<ProveedorPage>(),
                        "ModuloGrupo" => App.ServiceProvider.GetRequiredService<GrupoPage>(),
                        "ModuloProducto" => App.ServiceProvider.GetRequiredService<ProductoPage>(),
                        "ModuloMaquina" => App.ServiceProvider.GetRequiredService<MaquinaPage>(),
                        "ModuloOperador" => App.ServiceProvider.GetRequiredService<OperarioPage>(),
                        _ => null
                    };

                    if (page != null)
                    {
                        MainFrame.Navigate(page);
                    }
                    else
                    {
                        MessageBox.Show("Módulo no reconocido.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemCosto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                Page? page = item.Tag switch
                {
                    "ModuloImp" =>  App.ServiceProvider.GetRequiredService<ImpresionPage>(),
                    "ModuloEnc" =>  App.ServiceProvider.GetRequiredService<EncuadernacionPage>(),
                    "ModuloFot" =>  App.ServiceProvider.GetRequiredService<FotomecanicaPage>(),
                    _ => null
                };

                if (page != null)
                {
                    MainFrame.Navigate(page);
                }
                else
                {
                    MessageBox.Show("Módulo no reconocido.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
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
            var login = App.ServiceProvider.GetRequiredService<LoginWindow>();
            login.Show();

            this.Close();
        }
        

        private void BtnInicio_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new MainPage());
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
        public string Nombre { get; set; } = string.Empty;
        public required Func<Window> CrearVentana { get; set; } 
    }
}
