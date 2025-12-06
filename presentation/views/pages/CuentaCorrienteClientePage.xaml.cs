using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;

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

        Loaded += async (_, _) => await _viewModel.LoadAllByEmpresa();
        _dataGrid = dgClientes;
        _dataGrid.ItemContainerGenerator.StatusChanged += DataGrid_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }
    
    private async void BtnSaldos_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private async void BtnCartola_Click(object sender, RoutedEventArgs e)
    {


    }

    private async void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        
    }

    private async void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private async void BtnOT_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Filtro = txtBuscar.Text;
        _viewModel.Buscar(); 
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