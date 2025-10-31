using System.Windows;
using System.Windows.Input;
using Gestion.presentation.viewmodel;

namespace Gestion.presentation.views.windows
{
    public partial class ProductoModalPage : Window
    {
        private readonly ProductoViewModel _viewModel;
        public ProductoModalPage(ProductoViewModel productoViewModel)
        {
            InitializeComponent();
            _viewModel = productoViewModel;
            DataContext = _viewModel;
            Title = $"Productos";

            Loaded += ProductoModalPage_Loaded;
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
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert) BtnAgregar_Click(sender, e);
            else if (e.Key == Key.Delete) BtnEliminar_Click(sender, e);
            else if (e.Key == Key.Enter) BtnEditar_Click(sender, e);
            else if (e.Key == Key.F2) BtnBuscar_Click(sender, e);
            else if (e.Key == Key.F4) BtnImprimir_Click(sender, e);
        }

        private async void ProductoModalPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadProductos();
        }
    }
}
