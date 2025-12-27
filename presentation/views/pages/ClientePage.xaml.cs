using Gestion.core.model;
using Gestion.helpers;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.util;
using Gestion.presentation.views.windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        Loaded += async (_, _) =>
        {
            await _viewModel.LoadPageByEmpresa(1);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        paginacion.PageChanged += async (nuevaPagina) =>
        {
            await _viewModel.LoadPageByEmpresa(nuevaPagina);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        paginacion.PageSizeChanged += async (size) =>
        {
            _viewModel.PageSize = size;

            if (size == 0)
                await _viewModel.LoadAllByEmpresa(); // sin paginar
            else
                await _viewModel.LoadPageByEmpresa(1); // resetear a página 1

            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        _dataGrid = dgClientes;
        _dataGrid.ItemContainerGenerator.StatusChanged += DataGrid_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }

    private void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        EditorHelper.Abrir(
            owner: Window.GetWindow(this),
            entidad: new Cliente(),
            accion: async entidad => await _viewModel.Save((Cliente)entidad),
            titulo: "Agregar Cliente");
    }

    private void Editar(Cliente cliente)
    {
        EditorHelper.Abrir(
            owner: Window.GetWindow(this),
            entidad: cliente,
            accion: async entidad => await _viewModel.Update((Cliente)entidad),
            titulo: "Editar Cliente");
    }

    private void EditarSeleccionado()
    {
        if (dgClientes.SelectedItem is Cliente seleccionado)
            Editar(seleccionado);
    }

    private void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        EditarSeleccionado();
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        EditarSeleccionado();
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is Cliente seleccionado)
        {
            if (DialogUtils.Confirmar($"¿Seguro que deseas eliminar al cliente \"{seleccionado.Razon_Social}\"?", "Confirmar eliminación"))
            {
                await _viewModel.Delete(seleccionado.Id);
                DialogUtils.MostrarInfo("Cliente eliminado correctamente.", "Éxito");
            }
        }
        else
        {
            DialogUtils.MostrarAdvertencia("Selecciona un cliente antes de eliminar.", "Aviso");
        }
    }

    private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        string? filtro = txtBuscar.Text?.Trim();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            // Si hay texto, cargar TODO antes de filtrar
            _viewModel.PageSize = 0;
            await _viewModel.LoadAllByEmpresa();

            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        }
        else
        {
            // Si está vacío, volver a paginación normal
            if (_viewModel.PageSize == 0)
            {
                _viewModel.PageSize = paginacion.CurrentPageSize; // el valor del TextBox de paginación
            }

            await _viewModel.LoadPageByEmpresa(1);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        }

        if (filtro == null)
            return;

        _viewModel.Buscar(filtro);
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

    private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            BtnBuscar_Click(sender, e);
            e.Handled = true;
        }
    }
}