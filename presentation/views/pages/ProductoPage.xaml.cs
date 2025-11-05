using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages
{
    public partial class ProductoPage : Page
    {
        private readonly ProductoViewModel _viewModel;
        public ProductoPage(ProductoViewModel productoViewModel)
        {
            InitializeComponent();
            _viewModel = productoViewModel;
            DataContext = _viewModel;
            Title = $"Productos";

            Loaded += ProductoPage_Loaded;
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

        private async void dgProductos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgProductos.SelectedItem is Producto productoSeleccionado)
            {
                var ventana = new EntidadEditorWindow(productoSeleccionado)
                {
                    Title = "Editar Producto",
                };

                if (ventana.ShowDialog() == true)
                {
                    var productoEditado = (Producto)ventana.EntidadEditada;
                    await _viewModel.updateProducto(productoEditado);
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

        private async void ProductoPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadProductos();
        }
    }
}
