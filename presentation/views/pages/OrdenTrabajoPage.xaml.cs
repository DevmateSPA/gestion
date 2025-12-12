using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;
using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Gestion.core.interfaces.service;
using System.Collections.ObjectModel;
using Gestion.core.model.detalles;
using Gestion.core.session;
using Gestion.presentation.util;

namespace Gestion.presentation.views.pages;

public partial class OrdenTrabajoPage : Page
{
    private DataGrid _dataGrid; 

    private readonly OrdenTrabajoViewModel _viewModel;
    private readonly IDetalleOTService _detalleOTService;
    public OrdenTrabajoPage(OrdenTrabajoViewModel viewModel, IDetalleOTService detalleOTService)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _detalleOTService = detalleOTService;
        DataContext = _viewModel;
        Title = $"Ordenes de Trabajo";

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
        _dataGrid = dgOrdenTrabajo;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgOrdenTrabajo_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ordenTrabajo = new OrdenTrabajo();
        ordenTrabajo.Empresa = SesionApp.IdEmpresa;
        var ventana = new OrdenTrabajoDetallePage(this, ordenTrabajo);

        if (ventana.ShowDialog() != true)
            return; 

        var ordenTrabajoEditado = (OrdenTrabajo)ventana.EntidadEditada;

        await _viewModel.Save(ordenTrabajoEditado);
        await _viewModel.SincronizarDetalles([], ordenTrabajoEditado.Detalles, ordenTrabajoEditado);
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is OrdenTrabajo ordenTrabajoSeleccionado)
            await Editar(ordenTrabajoSeleccionado, "Editar Orden de Trabajo");
    }

    private async void dgOrdenTrabajo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is OrdenTrabajo ordenTrabajoSeleccionado)
            await Editar(ordenTrabajoSeleccionado, "Editar Orden de Trabajo");
    }

    private async Task Editar(OrdenTrabajo ordenTrabajo, string titulo)
    {
        if (ordenTrabajo == null)
            return;

        var detalles = await _detalleOTService.FindByFolio(ordenTrabajo.Folio);
        ordenTrabajo.Detalles = new ObservableCollection<DetalleOrdenTrabajo>(detalles);

        var ventana = new OrdenTrabajoDetallePage(this, ordenTrabajo);
        if (ventana.ShowDialog() != true)
        {
            return;
        }

        var ordenTrabajoEditada = (OrdenTrabajo)ventana.EntidadEditada;

        await _viewModel.Update(ordenTrabajoEditada);
        await _viewModel.SincronizarDetalles(detalles, ordenTrabajoEditada.Detalles, ordenTrabajoEditada);
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is OrdenTrabajo seleccionado)
        {
            string cliente = string.IsNullOrWhiteSpace(seleccionado.RutCliente)
                ? "(sin cliente)"
                : seleccionado.RutCliente;

            string fecha = seleccionado.Fecha != default
                ? seleccionado.Fecha.ToString("dd-MM-yyyy")
                : "(sin fecha)";

            string mensaje =
                $"¿Deseas eliminar la orden de trabajo:\n\n" +
                $"• Folio: {seleccionado.Folio}\n" +
                $"• Cliente: {cliente}\n" +
                $"• Fecha: {fecha}\n" +
                "Esta acción eliminará también todos sus detalles asociados, " +
                "incluyendo papeles, tintas, sacos, sobres y cualquier otra información relacionada.\n\n" +
                "Esta acción no se puede deshacer.";

            if (DialogUtils.Confirmar(mensaje, "Confirmación requerida"))
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

    private void DgOrdenTrabajo_StatusChanged(object? sender, EventArgs e)
    {
        PageUtils.GridFocus(_dataGrid,DgOrdenTrabajo_StatusChanged);
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

    private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            BtnBuscar_Click(sender, e);
            e.Handled = true;
        }
    }
}
