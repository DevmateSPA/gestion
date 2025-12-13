using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.views.util;

namespace Gestion.presentation.views.pages;
    public partial class CuentaCorrienteClientePage : Page
    {
    private readonly ClienteViewModel _viewModel;
    
    private DataGrid _dataGrid;
    
    public CuentaCorrienteClientePage(ClienteViewModel clienteViewModel)
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
    
    private void BtnSaldos_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void BtnCartola_Click(object sender, RoutedEventArgs e)
    {
        var parentWindow = Window.GetWindow(this);
        var cliente = dgClientes.SelectedItem as Cliente;  
        if (cliente == null) return;
        
        var modalFecha = new FechaModal(titulo: "Seleccione rango de fechas");
            modalFecha.Owner = parentWindow;

        bool? ok = modalFecha.ShowDialog();

        if (ok == true)
        {
            var desde = modalFecha.FechaDesde;
            var hasta = modalFecha.FechaHasta;
            if (desde == null || hasta == null)
            {
                MessageBox.Show("Debe seleccionar ambas fechas.");
                return;
            }
            var modal = new ClienteCartolaWindow(cliente, desde.Value, hasta.Value);
            modal.Owner = parentWindow;
            modal.ShowDialog();
        }

    }


    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        BtnCartola_Click(sender,e);
            
    }

    private void BtnOT_Click(object sender, RoutedEventArgs e)
    {
        var cliente = dgClientes.SelectedItem as Cliente;  
        if (cliente == null)
        {
            MessageBox.Show("Seleccione alguna fila");
            return;
        }
        
        
        var modal = new ClienteOTWindow(cliente,"","");
        var parentWindow = Window.GetWindow(this);
        modal.Owner = parentWindow;
        modal.ShowDialog();
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
                BtnCartola_Click(sender, e);
                break;

            case Key.F2:
                BtnBuscar_Click(sender, e);
                break;
            case Key.F3:
                BtnSaldos_Click(sender, e);
                break;
            case Key.F7:
                BtnOT_Click(sender, e);
                break;
            case Key.Escape:
                BtnCancelar_Click(sender, e);
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

    private void Page_KeyDown(object sender, KeyEventArgs e)
{
    switch (e.Key)
    {
        case Key.F2:
            BtnBuscar_Click(sender, e);
            e.Handled = true;
            break;

        case Key.F3:
            BtnSaldos_Click(sender, e);
            e.Handled = true;
            break;

        case Key.Enter:
            BtnCartola_Click(sender, e);
            e.Handled = true;
            break;

        case Key.F7:
            BtnOT_Click(sender, e);
            e.Handled = true;
            break;
    }
}
}