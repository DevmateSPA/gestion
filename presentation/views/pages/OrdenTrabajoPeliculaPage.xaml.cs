using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;

namespace Gestion.presentation.views.pages;

public partial class OrdenTrabajoPeliculaPage : Page
{
    private DataGrid _dataGrid;

    private readonly OrdenTrabajoPeliculaViewModel _viewModel;
    public OrdenTrabajoPeliculaPage(OrdenTrabajoPeliculaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Ordenes de Trabajo Pelicula";

        Loaded += async (_, _) => await _viewModel.LoadAll();
        _dataGrid = dgOrdenTrabajoPelicula;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgOrdenTrabajoPelicula_StatusChanged;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EntidadEditorWindow(this, new OrdenTrabajoPelicula(), "Ingresar Orden de Trabajo Pelicula");

        if (ventana.ShowDialog() == true)
        {
            var ordenTrabajoPeliculaEditado = (OrdenTrabajoPelicula)ventana.EntidadEditada;
            await _viewModel.Save(ordenTrabajoPeliculaEditado);
        }
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is OrdenTrabajoPelicula ordenTrabajoPeliculaSeleccionado)
            editar(ordenTrabajoPeliculaSeleccionado, "Editar Orden de Trabajo Pelicula");
    }

    private async void dgOrdenTrabajoPelicula_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is OrdenTrabajoPelicula ordenTrabajoPeliculaSeleccionado)
            editar(ordenTrabajoPeliculaSeleccionado, "Editar Orden de Trabajo");
    }

    private async void editar(OrdenTrabajoPelicula ordenTrabajoPelicula, string titulo)
    {
        var ventana = new EntidadEditorWindow(this, ordenTrabajoPelicula, titulo);

        if (ventana.ShowDialog() == true)
        {
            var ordenTrabajoPeliculaEditado = (OrdenTrabajoPelicula)ventana.EntidadEditada;
            await _viewModel.Update(ordenTrabajoPeliculaEditado);
        }
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is OrdenTrabajoPelicula seleccionado)
        {
            if (DialogUtils.Confirmar($"¿Seguro que deseas eliminar la orden de trabajo \"{seleccionado.Folio}\"?", "Confirmar eliminación"))
            {
                await _viewModel.Delete(seleccionado.Id);
                DialogUtils.MostrarInfo("Orden de trabajo eliminada correctamente.", "Éxito");
            }
        }
        else
        {
            DialogUtils.MostrarAdvertencia("Selecciona una orden de trabajo antes de eliminar.", "Aviso");
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

    private void DgOrdenTrabajoPelicula_StatusChanged(object? sender, EventArgs e)
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

                dataGrid.ItemContainerGenerator.StatusChanged -= DgOrdenTrabajoPelicula_StatusChanged;
            }
        }
    }

    private void dgOrdenTrabajoPelicula_PreviewKeyDown(object sender, KeyEventArgs e)
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
