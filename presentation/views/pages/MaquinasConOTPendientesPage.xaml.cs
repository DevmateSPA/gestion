using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.pages;
using Microsoft.Extensions.DependencyInjection;

namespace Gestion.presentation.views.windows;

public partial class MaquinasConOTPendientesPage : Page
{
    private readonly DataGrid _dataGrid;
    private readonly MaquinaViewModel _viewModel;
    public Maquina? MaquinaSeleccionada;
    public MaquinasConOTPendientesPage(MaquinaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

                Loaded += async (_, _) =>
        {
            await _viewModel.LoadPageMaquinaWithPendingOrders(1);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        paginacion.PageChanged += async (nuevaPagina) =>
        {
            await _viewModel.LoadPageMaquinaWithPendingOrders(nuevaPagina);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        paginacion.PageSizeChanged += async (size) =>
        {
            _viewModel.PageSize = size;

            if (size == 0)
                await _viewModel.LoadAllMaquinaWithPendingOrders(); // sin paginar
            else
                await _viewModel.LoadPageMaquinaWithPendingOrders(1); // resetear a página 1

            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };


        _dataGrid = dgMaquinas;

        // Asociar evento de búsqueda
        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }

 private async void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter) return;

        string? filtro = txtBuscar.Text?.Trim();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            // Si hay texto, cargar todo antes de filtrar
            _viewModel.PageSize = 0;
            await _viewModel.LoadAllMaquinaWithPendingOrders();

            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        }
        else
        {
            // Si está vacío, volver a paginación normal
            if (_viewModel.PageSize == 0)
                _viewModel.PageSize = paginacion.CurrentPageSize;

            await _viewModel.LoadPageMaquinaWithPendingOrders(1);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        }

        if (filtro != null)
            _viewModel.Buscar(filtro);
        e.Handled = true;
    }

    private void dgMaquinas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is Maquina maquinaSeleccionada)
        {
            this.MaquinaSeleccionada = maquinaSeleccionada;

            if (maquinaSeleccionada != null)
            {
                var ordenTrabajoVM = App.ServiceProvider.GetRequiredService<OrdenTrabajoViewModel>();
                Window window = new PendienteMaquinaWindow(ordenTrabajoVM, maquinaSeleccionada);
                window.ShowDialog();
            }
        }
            
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs  e)
    {
        
    }
}

