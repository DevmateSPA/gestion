using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.presentation.viewmodel;

namespace Gestion.presentation.views.pages
{
    public partial class ProveedorPage : Page
    {
        private readonly ProveedorViewModel _viewModel;
        public ProveedorPage(ProveedorViewModel proveedorViewModel)
        {
            InitializeComponent();
            _viewModel = proveedorViewModel;
            DataContext = _viewModel;
            Title = $"Proveedores";

            Loaded += ProveedorPage_Loaded;
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

        // Atajos de teclado
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert) BtnAgregar_Click(sender, e);
            else if (e.Key == Key.Delete) BtnEliminar_Click(sender, e);
            else if (e.Key == Key.Enter) BtnEditar_Click(sender, e);
            else if (e.Key == Key.F2) BtnBuscar_Click(sender, e);
            else if (e.Key == Key.F4) BtnImprimir_Click(sender, e);
        }

        private async void ProveedorPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadProveedores();
        }
    }
}
