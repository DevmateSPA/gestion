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
                Page? page = item.Tag switch
                {
                    "ModuloGuia" =>  App.ServiceProvider.GetRequiredService<GuiaDespachoPage>(),
                    "ModuloFactura" => App.ServiceProvider.GetRequiredService<FacturaPage>(),
                    "ModuloNotaCredito" => App.ServiceProvider.GetRequiredService<NotaCreditoPage>(),
                    "ModuloDocNulo" => App.ServiceProvider.GetRequiredService<DocumentoNuloPage>(),
                    "ModuloPagoAbono" => null,
                    "ModuloCancelacion" => null,
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

        private void MenuItemDocCom_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                Page? page = item.Tag switch
                {
                    "ModuloFactura" =>  App.ServiceProvider.GetRequiredService<FacturaCompraPage>(),
                    "ModuloNotaCredito" => null,
                    "ModuloPagoAbono" => null,
                    "ModuloCancelacion" => null,
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

        private void MenuItemControl_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem item)
            {
                Page? page = item.Tag switch
                {
                    "ModuloOrden" => App.ServiceProvider.GetRequiredService<OrdenTrabajoPage>(),
                    "ModuloOrdenPel" => null,
                    "ModuloSalida" => null,
                    "ModuloAjuste" => null,
                    "ModuloCalculo" => null,
                    "ModuloCotizacion" => null,
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
