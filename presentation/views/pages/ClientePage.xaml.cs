using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages;
    public partial class ClientePage : Page
    {
    private readonly ClienteViewModel _viewModel;
    
    private DataGrid _dataGrid;
    
    public ClientePage(ClienteViewModel clienteViewModel)
    {
        InitializeComponent();
        _viewModel = clienteViewModel;
        DataContext = _viewModel;
        Title = $"Clientes";

        Loaded += async (_, _) => await _viewModel.LoadAll();
        _dataGrid = dgClientes;
        _dataGrid.ItemContainerGenerator.StatusChanged += DataGrid_StatusChanged;
    }
    
    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EntidadEditorWindow(new Cliente(), "Ingresar Cliente");

        if (ventana.ShowDialog() == true)
        {
            var guardado = (Cliente)ventana.EntidadEditada;
            await _viewModel.Save(guardado);
        }
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is Cliente seleccionado)
            editar(seleccionado, "Editar Cliente");
    }

    private async void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is Cliente seleccionado)
            editar(seleccionado, "Editar Cliente");
    }

    private async void editar(Cliente cliente, string titulo)
    {
        var ventana = new EntidadEditorWindow(cliente, titulo);

        if (ventana.ShowDialog() == true)
        {
            var editado = (Cliente)ventana.EntidadEditada;
            await _viewModel.Update(editado);
        }
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is Cliente seleccionado)
            await _viewModel.Delete(seleccionado.Id);
    }

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show($"Buscar: {txtBuscar.Text}");
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Imprimir listado...");
    }

    private void DataGrid_StatusChanged(object? sender, EventArgs e)
    {
        GridFocus(_dataGrid);
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
                    firstRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                dataGrid.ItemContainerGenerator.StatusChanged -= DataGrid_StatusChanged;
            }
        }
    }

    private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var teclas = new[] { Key.Enter, Key.Insert, Key.Delete, Key.F2, Key.F4 };

        if (!teclas.Contains(e.Key))
            return;

        e.Handled = true;

        switch (e.Key)
        {
            case Key.Enter:
                BtnEditar_Click(sender, e);
                break;

            case Key.Insert:
                BtnAgregar_Click(sender, e);
                break;

            case Key.Delete:
                BtnEliminar_Click(sender, e);
                break;

            case Key.F2:
                BtnBuscar_Click(sender, e);
                break;

            case Key.F4:
                BtnImprimir_Click(sender, e);
                break;
        }
    }
}