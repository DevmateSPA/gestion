using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;
using Gestion.core.interfaces.service;
using System.Collections.ObjectModel;

namespace Gestion.presentation.views.pages;

public partial class FacturaCompraPage : Page
{
    private DataGrid _dataGrid;

    private readonly FacturaCompraViewModel _viewModel;
    private readonly IFacturaCompraProductoService _detalleService;
    public FacturaCompraPage(FacturaCompraViewModel viewModel, IFacturaCompraProductoService detalleService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _detalleService = detalleService;
        DataContext = _viewModel;
        Title = $"Facturas de Compra";

        Loaded += async (_, _) =>
        {
            await _viewModel.LoadAllByEmpresa();
        };
        _dataGrid = dgFacturasCompra;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgFacturasCompra_StatusChanged;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var factura = new FacturaCompra();
        var ventana = new EntidadEditorTableWindow(this, factura, factura.Detalles, "Agregar Factura");

        if (ventana.ShowDialog() == true)
        {
            var facturaEditado = (FacturaCompra)ventana.EntidadEditada;
            await _viewModel.Save(facturaEditado);
        }
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is FacturaCompra facturaCompraSeleccionado)
            editar(facturaCompraSeleccionado, "Editar Facturas de Compra");
    }

    private async void dgFacturasCompra_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is FacturaCompra facturaCompraSeleccionado)
            editar(facturaCompraSeleccionado, "Editar Facturas de Compra");
    }

    // Metodo Editar -------------------------------------------- |
    private async void editar(FacturaCompra factura, string titulo)
    {
        if (factura == null)
            return;

        var detalles = await _detalleService.FindByFolio(factura.Folio);
        factura.Detalles = new ObservableCollection<FacturaCompraProducto>(detalles);

        var ventana = new EntidadEditorTableWindow(this, factura, factura.Detalles, titulo);

        if (ventana.ShowDialog() != true)
        {
            var facturaCancelada = (FacturaCompra)ventana.EntidadEditada;
            return;
        }

        var facturaEditada = (FacturaCompra)ventana.EntidadEditada;

        await _viewModel.Update(facturaEditada);
        factura.Detalles?.Clear();
    }

    // ------------------------------------------------------ |

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is FacturaCompra seleccionado)
        {
            if (DialogUtils.Confirmar($"¿Seguro que deseas eliminar la factura \"{seleccionado.Id}\"?", "Confirmar eliminación"))
            {
                await _viewModel.Delete(seleccionado.Id);
                DialogUtils.MostrarInfo("Factura eliminada correctamente.", "Éxito");
            }
        }
        else
        {
            DialogUtils.MostrarAdvertencia("Selecciona una factura antes de eliminar.", "Aviso");
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

    private void DgFacturasCompra_StatusChanged(object? sender, EventArgs e)
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
                {
                    firstRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }

                dataGrid.ItemContainerGenerator.StatusChanged -= DgFacturasCompra_StatusChanged;
            }
        }
    }

    private void dgFacturasCompra_PreviewKeyDown(object sender, KeyEventArgs e)
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