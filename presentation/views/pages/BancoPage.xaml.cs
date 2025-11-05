using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Microsoft.Extensions.DependencyInjection;

namespace Gestion.presentation.views.pages
{
    public partial class BancoPage : Page
    {
        private readonly BancoViewModel _viewModel;
        public BancoPage(BancoViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Title = $"Bancos";

            Loaded += BancoPage_Loaded;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var ventana = App.ServiceProvider.GetRequiredService<AgregarBancoWindow>();
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();

            string codigo = ventana.Codigo;
            string nombre = ventana.Nombre;
            string direccion = ventana.Direccion;
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar banco...");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Editar banco...");
        }

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Buscar: {txtBuscar.Text}");
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Imprimir listado...");
        }
        
        private async void dgBancos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgBancos.SelectedItem is Banco bancoSeleccionado)
            {
                var ventana = new EntidadEditorWindow(bancoSeleccionado)
                {
                    Title = "Editar Banco"
                };

                if (ventana.ShowDialog() == true)
                {
                    var bancoEditado = (Banco)ventana.EntidadEditada;
                    await _viewModel.updateBanco(bancoEditado);
                }
            }
        }

        // Atajos de teclado
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert) BtnAgregar_Click(sender, e);
            else if (e.Key == Key.Delete) BtnEliminar_Click(sender, e);
            else if (e.Key == Key.Enter) BtnEditar_Click(sender, e);
            else if (e.Key == Key.F2) BtnBuscar_Click(sender, e);
            else if (e.Key == Key.F4) BtnImprimir_Click(sender, e);
        }

        private async void BancoPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadBancos();
        }
    }
}
