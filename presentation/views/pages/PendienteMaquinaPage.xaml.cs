using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.views.util;

namespace Gestion.presentation.views.pages;
    public partial class PendienteMaquinaPage : Page
    {
    private readonly OrdenTrabajoViewModel _viewModel;
    private readonly Maquina _maquina;
    
    private DataGrid _dataGrid;
    
    public PendienteMaquinaPage(OrdenTrabajoViewModel ordenTrabajoViewModel, Maquina maquina)
    {
        InitializeComponent();
        _viewModel = ordenTrabajoViewModel;
        _maquina = maquina;
        DataContext = _viewModel;
        Title = $"Pendientes Maquina";

        Loaded += async (_, _) =>
        {
            await _viewModel.LoadPageByMaquinaWhereEmpresaAndPendiente(1, _maquina.Codigo);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        paginacion.PageChanged += async (nuevaPagina) =>
        {
            await _viewModel.LoadPageByMaquinaWhereEmpresaAndPendiente(nuevaPagina, _maquina.Codigo);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        paginacion.PageSizeChanged += async (size) =>
        {
            _viewModel.PageSize = size;

            if (size == 0)
                await _viewModel.LoadAllByMaquinaWhereEmpresaAndPendiente(_maquina.Codigo); // sin paginar
            else
                await _viewModel.LoadPageByMaquinaWhereEmpresaAndPendiente(1, _maquina.Codigo); // resetear a página 1

            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };
        _dataGrid = dgOrdenesTrabajo;
        _dataGrid.ItemContainerGenerator.StatusChanged += DataGrid_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }
    
    private void BtnSaldos_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void BtnCartola_Click(object sender, RoutedEventArgs e)
    {


    }

    private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void BtnOT_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        string? filtro = txtBuscar.Text?.Trim();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            // Si hay texto, cargar TODO antes de filtrar
            _viewModel.PageSize = 0;
            await _viewModel.LoadAllByMaquinaWhereEmpresaAndPendiente(_maquina.Codigo);

            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        }
        else
        {
            // Si está vacío, volver a paginación normal
            if (_viewModel.PageSize == 0)
            {
                _viewModel.PageSize = paginacion.CurrentPageSize; // el valor del TextBox de paginación
            }

            await _viewModel.LoadPageByMaquinaWhereEmpresaAndPendiente(1, _maquina.Codigo);
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
}