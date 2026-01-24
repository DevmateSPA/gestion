using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.views.util;
using Gestion.core.interfaces.service;
using System.Collections.ObjectModel;
using Gestion.helpers;

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
        _dataGrid = dgFacturasCompra;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgFacturasCompra_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        await new EditorEntidadBuilder<FacturaCompra>()
            .Owner(Window.GetWindow(this)!)
            .Entidad(new FacturaCompra())
            .Titulo("Agregar Factura de Compra")
            .Guardar(_viewModel.Save)
            .OnClose(async facturaEditada =>
                await _viewModel.SincronizarDetalles(
                    [],
                    facturaEditada.Detalles.Cast<FacturaCompraProducto>(),
                    facturaEditada))
            .Abrir();
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is FacturaCompra facturaCompraSeleccionado)
            await Editar(facturaCompraSeleccionado);
    }

    private async void dgFacturasCompra_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is FacturaCompra facturaCompraSeleccionado)
            await Editar(facturaCompraSeleccionado);
    }

    // Metodo Editar -------------------------------------------- |
    private async Task Editar(FacturaCompra factura, string titulo = "Editar Factura de Compra")
    {
        if (factura == null)
            return;

        var detallesOriginales = await _viewModel.LoadDetailsByFolio(factura.Folio, factura.Empresa);

        factura.Detalles = new ObservableCollection<FacturaCompraProducto>(
            detallesOriginales.Select(d => (FacturaCompraProducto)d.Clone()));

        await new EditorEntidadBuilder<FacturaCompra>()
            .Owner(Window.GetWindow(this)!)
            .Entidad(factura)
            .Titulo(titulo)
            .Guardar(async f =>
            {
                await _viewModel.Update(f);
                return true;
            })
            .OnClose(async f =>
                await _viewModel.SincronizarDetalles(
                    detallesOriginales,
                    f.Detalles,
                    f))
            .Abrir();
    }

    // ------------------------------------------------------ |

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is FacturaCompra seleccionado)
        {
            if (DialogUtils.Confirmar(
                $"¿Deseas eliminar la factura \"{seleccionado.Id}\" del proveedor {seleccionado.RutCliente}?\n\n" +
                "Ten en cuenta que esta acción también removerá todos sus datos relacionados.",
                "Confirmación requerida"))
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