using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages;

public partial class NotaCreditoPage : Page
{
    private readonly NotaCreditoViewModel _viewModel;
    public NotaCreditoPage(NotaCreditoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Notas de crédito";

        Loaded += async (_, _) => await _viewModel.LoadAll();
        dgNotasCredito.ItemContainerGenerator.StatusChanged += DgNotasCredito_StatusChanged;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EntidadEditorWindow(new NotaCredito(), "Ingresar Nota de crédito");

        if (ventana.ShowDialog() == true)
        {
            var notaCreditoEditado = (NotaCredito)ventana.EntidadEditada;
            await _viewModel.Save(notaCreditoEditado);
        }
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (dgNotasCredito.SelectedItem is NotaCredito notaCreditoSeleccionado)
            editar(notaCreditoSeleccionado, "Editar Nota credito");
    }

    private async void dgNotasCredito_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dgNotasCredito.SelectedItem is NotaCredito notaCreditoSeleccionado)
            editar(notaCreditoSeleccionado, "Editar Nota credito");
    }

    private async void editar(NotaCredito notaCredito, string titulo)
    {
        var ventana = new EntidadEditorWindow(notaCredito, titulo);

        if (ventana.ShowDialog() == true)
        {
            var notaCreditoEditado = (NotaCredito)ventana.EntidadEditada;
            await _viewModel.Update(notaCreditoEditado);
        }
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (dgNotasCredito.SelectedItem is NotaCredito notaCreditoSeleccionado)
            await _viewModel.Delete(notaCreditoSeleccionado.Id);
    }

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show($"Buscar: {txtBuscar.Text}");
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Imprimir listado...");
    }

    private void DgNotasCredito_StatusChanged(object? sender, EventArgs e)
    {
        GridFocus(dgNotasCredito);
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

                dataGrid.ItemContainerGenerator.StatusChanged -= DgNotasCredito_StatusChanged;
            }
        }
    }

    private void dgNotasCredito_PreviewKeyDown(object sender, KeyEventArgs e)
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
