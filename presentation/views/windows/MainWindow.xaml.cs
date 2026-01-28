using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.viewmodel;
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

            if (SesionApp.TipoUsuario != "admin")
            {
                menuUsuarios.Visibility = Visibility.Collapsed;
            }

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
                    string tituloVentana = item.Tag switch
                    {
                        "ModuloBanco" => "Bancos",
                        "ModuloCliente" => "Clientes",
                        "ModuloProveedor" => "Proveedores",
                        "ModuloGrupo" => "Grupos",
                        "ModuloProducto" => "Productos",
                        "ModuloMaquina" => "Máquinas",
                        "ModuloOperador" => "Operadores",
                        _ => "Módulo desconocido"
                    };

                    var window = Window.GetWindow(this);

                    if (window != null)
                    {
                        window.Title = "Gestión — " + tituloVentana;
                    }
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
                string tituloVentana = item.Tag switch
                {
                    "ModuloImp" => "Impresiones",
                    "ModuloEnc" => "Encuadernaciones",
                    "ModuloFot" => "Fotomecanica",
                    _ => "Módulo desconocido"
                };
                var window = Window.GetWindow(this);

                if (window != null)
                {
                    window.Title = "Gestión — " + tituloVentana;
                }
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
                string tituloVentana = item.Tag switch
                {
                    "ModuloGuia" => "Guías de Despacho",
                    "ModuloFactura" => "Facturas",
                    "ModuloNotaCredito" => "Notas de Crédito",
                    "ModuloDocNulo" => "Documentos Nulos",
                    "ModuloVentas" => "Ventas",
                    _ => "Módulo desconocido"
                };

                var window = Window.GetWindow(this);

                if (window != null)
                {
                    window.Title = "Gestión — " + tituloVentana;
                }
                Page? page = item.Tag switch
                {
                    "ModuloGuia" =>  App.ServiceProvider.GetRequiredService<GuiaDespachoPage>(),
                    "ModuloFactura" => App.ServiceProvider.GetRequiredService<FacturaPage>(),
                    "ModuloNotaCredito" => App.ServiceProvider.GetRequiredService<NotaCreditoPage>(),
                    "ModuloDocNulo" => App.ServiceProvider.GetRequiredService<DocumentoNuloPage>(),
                    "ModuloVentas" => App.ServiceProvider.GetRequiredService<VentaPage>(),
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
                string tituloVentana = item.Tag switch
                {
                    "ModuloFactura" => "Facturas de Compra",
                    _ => "Módulo desconocido"
                };

                var window = Window.GetWindow(this);

                if (window != null)
                {
                    window.Title = "Gestión — " + tituloVentana;
                }
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
                string tituloVentana = item.Tag switch
                {
                    "ModuloOrden" => "Ordenes de Trabajo",
                    _ => "Módulo desconocido"
                };

                var window = Window.GetWindow(this);

                if (window != null)
                {
                    window.Title = "Gestión — " + tituloVentana;
                }
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

        private void MenuItemSalida_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                {
                    window.Title = "Gestión — Cuentas Corrientes Clientes";
                }
            Page? page = App.ServiceProvider.GetRequiredService<CuentaCorrienteClientePage>();
            MainFrame.Navigate(page);
        }

        private void MenuItemReportes_Click(object sender, RoutedEventArgs e)
        {
            Page? page = null;

            if (sender is MenuItem item)
            {
                if (item.Tag as string == "ModuloIngresosCliente")
                {
                    var modalFecha = new FechaModal();
                    modalFecha.Owner = Window.GetWindow(this);

                    if (modalFecha.ShowDialog() == true)
                    {
                        var desde = modalFecha.FechaDesde;
                        var hasta = modalFecha.FechaHasta;

                        if (desde == null || hasta == null)
                        {
                            MessageBox.Show("Debe seleccionar ambas fechas.");
                            return;
                        }

                        page = new IngresoClientePage(
                            App.ServiceProvider.GetRequiredService<IngresoClienteViewModel>(),
                            desde.Value,
                            hasta.Value
                        );
                    }
                }
                else if (item.Tag as string == "ModuloMaquina")
                {
                    page = App.ServiceProvider.GetRequiredService<MaquinasConOTPendientesPage>();
                }
                else
                {
                    page = item.Tag switch
                    {
                        "ModuloProduccion" => App.ServiceProvider.GetRequiredService<PendienteProduccionPage>(),
                        _ => null
                    };
                }

                if (page != null)
                    MainFrame.Navigate(page);
                else
                    MessageBox.Show("Módulo no reconocido.");
            }
        }

        private void MenuItemImpresora_Click(object sender, RoutedEventArgs e)
        {
            var modal = new ImpresoraModal
            {
                Owner = this 
            };

            if (modal.ShowDialog() == true)
            {
                string impresora = modal.ImpresoraSeleccionada;
                MessageBox.Show("Impresora seleccionada: " + impresora);
            }
        }

        private void MenuItemUsuarios_Click(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                {
                    window.Title = "Gestión — Usuarios";
                }
            Page? page = App.ServiceProvider.GetRequiredService<UsuariosPage>();
            MainFrame.Navigate(page);
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
        private void BtnOT_Click(object sender, RoutedEventArgs e)
        {
            Page? page = App.ServiceProvider.GetRequiredService<OrdenTrabajoPage>();
            MainFrame.Navigate(page);
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
