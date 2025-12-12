using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;

namespace Gestion.presentation.views.pages;

public partial class GuiaDespachoPage : Page
{
    private DataGrid _dataGrid;

    private readonly GuiaDespachoViewModel _viewModel;
    public GuiaDespachoPage(GuiaDespachoViewModel viewModel) 
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Guías de Despacho";

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

        _dataGrid = dgGuiasDespacho;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgGuiasDespacho_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var guiaDespacho = new GuiaDespacho();
        var ventana = new EntidadEditorWindow(this, guiaDespacho, "Ingresar Guía de despacho");

        if (ventana.ShowDialog() != true)
            return; 

        var guiaDespachoEditado = (GuiaDespacho)ventana.EntidadEditada;

        await _viewModel.Save(guiaDespachoEditado);
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (dgGuiasDespacho.SelectedItem is GuiaDespacho guiaDespachoSeleccionado)
            await editar(guiaDespachoSeleccionado, "Editar Guias de Despacho");
    }

    private async void dgGuiasDespacho_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dgGuiasDespacho.SelectedItem is GuiaDespacho guiaDespachoSeleccionado)
            await editar(guiaDespachoSeleccionado, "Editar Guias de Despacho");
    }

    private async Task editar(GuiaDespacho guiaDespacho, string titulo)
    {
        if (guiaDespacho == null)
            return;

        var ventana = new EntidadEditorWindow(this, guiaDespacho, titulo);

        if (ventana.ShowDialog() != true)
        {
            var guiaDespachoCancelada = (GuiaDespacho)ventana.EntidadEditada;
            return;
        }

        var guiaDespachoEditada = (GuiaDespacho)ventana.EntidadEditada;

        await _viewModel.Update(guiaDespachoEditada);
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is GuiaDespacho seleccionado)
        {
            if (DialogUtils.Confirmar($"¿Seguro que deseas eliminar la guía \"{seleccionado.Folio}\"?", "Confirmar eliminación"))
            {
                await _viewModel.Delete(seleccionado.Id);
                DialogUtils.MostrarInfo("Guía eliminado correctamente.", "Éxito");
            }
        }
        else
        {
            DialogUtils.MostrarAdvertencia("Selecciona una guía antes de eliminar.", "Aviso");
        }
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

    private void dgGuiasDespacho_PreviewKeyDown(object sender, KeyEventArgs e)
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
