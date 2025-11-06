using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages
{
    public partial class ClientePage : Page
    {
        private readonly ClienteViewModel _viewModel;
        public ClientePage(ClienteViewModel clienteViewModel)
        {
            InitializeComponent();
            _viewModel = clienteViewModel;
            DataContext = _viewModel;
            Title = $"Clientes";

            Loaded += ClientePage_Loaded;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Agregar banco...");
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

        private async void dgClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgClientes.SelectedItem is Cliente clienteSeleccionado)
            {
                var ventana = new EntidadEditorWindow(clienteSeleccionado)
                {
                    Title = "Editar Cliente"
                };

                if (ventana.ShowDialog() == true)
                {
                    var clienteEditado = (Cliente)ventana.EntidadEditada;
                    await _viewModel.updateCliente(clienteEditado);
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

        private async void ClientePage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadClientes();
        }
    }
}
