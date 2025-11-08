using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages;

public partial class GuiaDespachoPage : Page
{
    private readonly GuiaDespachoViewModel _viewModel;
    public GuiaDespachoPage(GuiaDespachoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Guías de Despacho";

        Loaded += async (_, _) => await _viewModel.LoadAll();
        dgGuiasDespacho.ItemContainerGenerator.StatusChanged += DgGuiasDespacho_StatusChanged;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EntidadEditorWindow(new GuiaDespacho(), "Ingresar GuiaDespacho");

        if (ventana.ShowDialog() == true)
        {
            var guiaDespachoEditado = (GuiaDespacho)ventana.EntidadEditada;
            await _viewModel.Save(guiaDespachoEditado);
        }
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (dgGuiasDespacho.SelectedItem is GuiaDespacho guiaDespachoSeleccionado)
            editar(guiaDespachoSeleccionado, "Editar Guias de Despacho");
    }

    private async void dgGuiaDespacho_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dgGuiasDespacho.SelectedItem is GuiaDespacho guiaDespachoSeleccionado)
            editar(guiaDespachoSeleccionado, "Editar Guias de Despacho");
    }

    private async void editar(GuiaDespacho guiaDespacho, string titulo)
    {
        var ventana = new EntidadEditorWindow(guiaDespacho, titulo);

        if (ventana.ShowDialog() == true)
        {
            var guiaDespachoEditado = (GuiaDespacho)ventana.EntidadEditada;
            await _viewModel.Update(guiaDespachoEditado);
        }
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (dgGuiasDespacho.SelectedItem is GuiaDespacho guiaDespachoSeleccionado)
            await _viewModel.Delete(guiaDespachoSeleccionado.Id);
    }

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show($"Buscar: {txtBuscar.Text}");
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Imprimir listado...");
    }

    private void DgGuiasDespacho_StatusChanged(object? sender, EventArgs e)
    {
        GridFocus(dgGuiasDespacho);
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

                dataGrid.ItemContainerGenerator.StatusChanged -= DgGuiasDespacho_StatusChanged;
            }
        }
    }

    private void dgGuiaDespacho_PreviewKeyDown(object sender, KeyEventArgs e)
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
