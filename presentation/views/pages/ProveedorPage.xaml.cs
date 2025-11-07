using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

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
            dgProveedores.ItemContainerGenerator.StatusChanged += DgProveedores_StatusChanged;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Agregar proveedor...");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar proveedor...");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgProveedores.SelectedItem is Proveedor proveedorSeleccionado)
            {
                var ventana = new EntidadEditorWindow(proveedorSeleccionado)
                {
                    Title = "Editar Proveedor"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(dgProveedores);
                    //var proveedorEditado = (Proveedore)ventana.EntidadEditada;
                    //await _viewModel.updateProveedore(proveedorEditado);
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

        private async void dgProveedores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgProveedores.SelectedItem is Proveedor proveedorSeleccionado)
            {
                var ventana = new EntidadEditorWindow(proveedorSeleccionado)
                {
                    Title = "Editar Proveedor",
                };

                if (ventana.ShowDialog() == true)
                {
                    var proveedorEditado = (Proveedor)ventana.EntidadEditada;
                    await _viewModel.update(proveedorEditado);
                }
            }
        }

        private async void dgProveedores_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void DgProveedores_StatusChanged(object? sender, EventArgs e)
        {
            GridFocus(dgProveedores);
        }

        private async void ProveedorPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadProveedores();
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgProveedores_StatusChanged;
                }
            }
        }
    }
}
