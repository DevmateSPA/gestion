using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.views.util;

namespace Gestion.presentation.views.pages;
    public partial class PendienteProduccionPage : Page
    {
    private readonly OrdenTrabajoViewModel _viewModel;
    
    private DataGrid _dataGrid;
    
    public PendienteProduccionPage(OrdenTrabajoViewModel ordenTrabajoViewModel)
    {
        InitializeComponent();
        _viewModel = ordenTrabajoViewModel;
        DataContext = _viewModel;
        Title = $"Pendientes Producción";

        Loaded += async (_, _) => await _viewModel.LoadAllByEntrega();
        _dataGrid = dgOrdenesTrabajo;
        _dataGrid.ItemContainerGenerator.StatusChanged += DataGrid_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }
    
    private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        
    }

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Buscar(txtBuscar.Text); 
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