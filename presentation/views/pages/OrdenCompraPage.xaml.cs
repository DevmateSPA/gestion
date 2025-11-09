using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages;

public partial class OrdenTrabajoPage : Page
{
    private readonly OrdenTrabajoViewModel _viewModel;
    public OrdenTrabajoPage(OrdenTrabajoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Ordenes de Trabajo";

        Loaded += async (_, _) => await _viewModel.LoadAll();
        dgOrdenTrabajo.ItemContainerGenerator.StatusChanged += DgOrdenTrabajo_StatusChanged;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EntidadEditorWindow(new OrdenTrabajo(), "Ingresar Orden de Trabajo");

        if (ventana.ShowDialog() == true)
        {
            var ordenTrabajoEditado = (OrdenTrabajo)ventana.EntidadEditada;
            await _viewModel.Save(ordenTrabajoEditado);
        }
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (dgOrdenTrabajo.SelectedItem is OrdenTrabajo ordenTrabajoSeleccionado)
            editar(ordenTrabajoSeleccionado, "Editar Orden de Trabajo");
    }

    private async void dgOrdenTrabajo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dgOrdenTrabajo.SelectedItem is OrdenTrabajo ordenTrabajoSeleccionado)
            editar(ordenTrabajoSeleccionado, "Editar Orden de Trabajo");
    }

    private async void editar(OrdenTrabajo ordenTrabajo, string titulo)
    {
        var ventana = new EntidadEditorWindow(ordenTrabajo, titulo);

        if (ventana.ShowDialog() == true)
        {
            var ordenTrabajoEditado = (OrdenTrabajo)ventana.EntidadEditada;
            await _viewModel.Update(ordenTrabajoEditado);
        }
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (dgOrdenTrabajo.SelectedItem is OrdenTrabajo ordenTrabajoSeleccionado)
            await _viewModel.Delete(ordenTrabajoSeleccionado.Id);
    }

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show($"Buscar: {txtBuscar.Text}");
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Imprimir listado...");
    }

    private void DgOrdenTrabajo_StatusChanged(object? sender, EventArgs e)
    {
        GridFocus(dgOrdenTrabajo);
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

                dataGrid.ItemContainerGenerator.StatusChanged -= DgOrdenTrabajo_StatusChanged;
            }
        }
    }

    private void dgOrdenTrabajo_PreviewKeyDown(object sender, KeyEventArgs e)
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
