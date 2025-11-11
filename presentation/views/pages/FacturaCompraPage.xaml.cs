using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;

namespace Gestion.presentation.views.pages;

public partial class FacturaCompraPage : Page
{
    private DataGrid _dataGrid;

    private readonly FacturaCompraViewModel _viewModel;
    public FacturaCompraPage(FacturaCompraViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Facturas de Compra";

        Loaded += async (_, _) => await _viewModel.LoadAll();
        _dataGrid = dgFacturasCompra;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgFacturasCompra_StatusChanged;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EntidadEditorWindow(this, new FacturaCompra(), "Ingresar Facturas de compra");

        if (ventana.ShowDialog() == true)
        {
            var facturaCompraEditado = (FacturaCompra)ventana.EntidadEditada;
            await _viewModel.Save(facturaCompraEditado);
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

    private async void editar(FacturaCompra factura, string titulo)
    {
        var ventana = new EntidadEditorWindow(this, factura, titulo);

        if (ventana.ShowDialog() == true)
        {
            var facturaEditado = (FacturaCompra)ventana.EntidadEditada;
            await _viewModel.Update(facturaEditado);
        }
    }

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
