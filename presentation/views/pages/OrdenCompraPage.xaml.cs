using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages;

public partial class OrdenCompraPage : Page
{
    private readonly OrdenCompraViewModel _viewModel;
    public OrdenCompraPage(OrdenCompraViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Ordenes de Compra";

        Loaded += async (_, _) => await _viewModel.LoadAll();
        dgOrdenCompra.ItemContainerGenerator.StatusChanged += DgOrdenCompra_StatusChanged;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EntidadEditorWindow(new OrdenCompra(), "Ingresar Orden de Compra");

        if (ventana.ShowDialog() == true)
        {
            var ordenCompraEditado = (OrdenCompra)ventana.EntidadEditada;
            await _viewModel.Save(ordenCompraEditado);
        }
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (dgOrdenCompra.SelectedItem is OrdenCompra ordenCompraSeleccionado)
            editar(ordenCompraSeleccionado, "Editar Orden de Compra");
    }

    private async void dgOrdenCompra_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dgOrdenCompra.SelectedItem is OrdenCompra ordenCompraSeleccionado)
            editar(ordenCompraSeleccionado, "Editar Orden de Compra");
    }

    private async void editar(OrdenCompra ordenCompra, string titulo)
    {
        var ventana = new EntidadEditorWindow(ordenCompra, titulo);

        if (ventana.ShowDialog() == true)
        {
            var ordenCompraEditado = (OrdenCompra)ventana.EntidadEditada;
            await _viewModel.Update(ordenCompraEditado);
        }
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (dgOrdenCompra.SelectedItem is OrdenCompra ordenCompraSeleccionado)
            await _viewModel.Delete(ordenCompraSeleccionado.Id);
    }

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show($"Buscar: {txtBuscar.Text}");
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Imprimir listado...");
    }

    private void DgOrdenCompra_StatusChanged(object? sender, EventArgs e)
    {
        GridFocus(dgOrdenCompra);
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

                dataGrid.ItemContainerGenerator.StatusChanged -= DgOrdenCompra_StatusChanged;
            }
        }
    }

    private void dgOrdenCompra_PreviewKeyDown(object sender, KeyEventArgs e)
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
