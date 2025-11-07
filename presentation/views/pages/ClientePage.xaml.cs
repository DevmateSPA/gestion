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
            dgClientes.ItemContainerGenerator.StatusChanged += DgClientes_StatusChanged;
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
            if (dgClientes.SelectedItem is Cliente clienteSeleccionado)
            {
                var ventana = new EntidadEditorWindow(clienteSeleccionado)
                {
                    Title = "Editar Cliente"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(dgClientes);
                    //var clienteEditado = (Cliente)ventana.EntidadEditada;
                    //await _viewModel.updateCliente(clienteEditado);
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
                    await _viewModel.Update(clienteEditado);
                }
            }
        }

         private async void dgClientes_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void DgClientes_StatusChanged(object? sender, EventArgs e)
        {
            GridFocus(dgClientes);
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgClientes_StatusChanged;
                }
            }
        }
    }
}
