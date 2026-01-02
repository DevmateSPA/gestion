using Gestion.core.model;
using Gestion.helpers;
using Gestion.presentation.viewmodel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gestion.presentation.views.pages;

public partial class NotaCreditoPage : Page
{
    private DataGrid _dataGrid;

    private readonly NotaCreditoViewModel _viewModel;
    public NotaCreditoPage(NotaCreditoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Notas de crédito";

        Loaded += async (_, _) =>
        {
            await _viewModel.LoadPageByEmpresa(1);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        paginacion.PageChanged += async (nuevaPagina) =>
        {
            await _viewModel.LoadPageByEmpresa(nuevaPagina);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        paginacion.PageSizeChanged += async (size) =>
        {
            _viewModel.PageSize = size;

            if (size == 0)
                await _viewModel.LoadAllByEmpresa(); // sin paginar
            else
                await _viewModel.LoadPageByEmpresa(1); // resetear a página 1

            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };
        _dataGrid = dgNotasCredito;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgNotasCredito_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        await new EditorEntidadBuilder<NotaCredito>()
            .Owner(Window.GetWindow(this)!)
            .Entidad(new NotaCredito())
            .Titulo("Agregar Nota de Crédito")
            .Guardar(_viewModel.Save)
            .Abrir();
    }

    private async Task Editar(NotaCredito notaCredito)
    {
        await new EditorEntidadBuilder<NotaCredito>()
            .Owner(Window.GetWindow(this)!)
            .Entidad(notaCredito)
            .Titulo("Editar Nota de Crédito")
            .Guardar(_viewModel.Update)
            .Abrir();
    }

    private async Task EditarSeleccionado()
    {
        if (dgNotasCredito.SelectedItem is NotaCredito notaCreditoSeleccionada)
            await Editar(notaCreditoSeleccionada);
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        await EditarSeleccionado();
    }

    private async void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        await EditarSeleccionado();
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        await EditorHelper.BorrarSeleccionado(
            seleccionado: _dataGrid.SelectedItem as NotaCredito,
            borrarAccion: async b => await _viewModel.Delete(b.Id),
            mensajeConfirmacion: $"¿Seguro que deseas eliminar la Nota de Crédito \"{((_dataGrid.SelectedItem as NotaCredito)?.Folio)}\"?",
            mensajeExito: "Nota de Crédito eliminada correctamente.");
    }

    private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        string? filtro = txtBuscar.Text?.Trim();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            // Si hay texto, cargar TODO antes de filtrar
            _viewModel.PageSize = 0;
            await _viewModel.LoadAllByEmpresa();

            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        }
        else
        {
            // Si está vacío, volver a paginación normal
            if (_viewModel.PageSize == 0)
            {
                _viewModel.PageSize = paginacion.CurrentPageSize; // el valor del TextBox de paginación
            }

            await _viewModel.LoadPageByEmpresa(1);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        }

        if (filtro == null)
            return;

        _viewModel.Buscar(filtro);
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Imprimir listado...");
    }

    private void DgNotasCredito_StatusChanged(object? sender, EventArgs e)
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

    private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            BtnBuscar_Click(sender, e);
            e.Handled = true;
        }
    }
}
