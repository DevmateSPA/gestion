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
            dgProductos.ItemContainerGenerator.StatusChanged += DgProductos_StatusChanged;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Agregar producto...");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar producto...");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem is Producto productoSeleccionado)
            {
                var ventana = new EntidadEditorWindow(productoSeleccionado)
                {
                    Title = "Editar Producto"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(dgProductos);
                    //var productoEditado = (Producto)ventana.EntidadEditada;
                    //await _viewModel.updateProducto(productoEditado);
                }
            }
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
                    await _viewModel.Update(productoEditado);
                }
            }
        }

        private async void dgProductos_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var teclas = new[] { Key.Enter, Key.Insert, Key.Delete, Key.F2, Key.F4 };
            if (teclas.Contains(e.Key))
            {
                e.Handled = true;
            }
            if (e.Key == Key.Enter) BtnEditar_Click(sender, e);
            else if (e.Key == Key.Insert) BtnAgregar_Click(sender, e);
            else if (e.Key == Key.Delete) BtnEliminar_Click(sender, e);
            else if (e.Key == Key.F2) BtnBuscar_Click(sender, e);
            else if (e.Key == Key.F4) BtnImprimir_Click(sender, e);
        }

        private void DgProductos_StatusChanged(object? sender, EventArgs e)
        {
            GridFocus(dgProductos);
        }

        private async void ProductoPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAll();
        }

        private void GridFocus(DataGrid dataGrid)
        {
            if (dataGrid.ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
            {
                if (dataGrid.Items.Count > 0)
                {
                    dataGrid.SelectedIndex = 0;
                    dataGrid.Focus();

                    var firstRow = dataGrid.ItemContainerGenerator.ContainerFromIndex(0) as DataGridRow;
                    if (firstRow != null)
                    {
                        firstRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    }

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgProductos_StatusChanged;
                }
            }
        }
    }
}
