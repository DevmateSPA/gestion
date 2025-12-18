using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;

namespace Gestion.presentation.views.windows;

public partial class MaquinasConOTPendientesWindow : Window
{
    private readonly DataGrid _dataGrid;
    private readonly MaquinaViewModel _viewModel;
    public Maquina? MaquinaSeleccionada;
    public MaquinasConOTPendientesWindow(MaquinaViewModel viewModel)
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

        // Cancelar el cierre
        Closing += MaquinasConOTPendientesWindow_Closing;

        _dataGrid = dgMaquinas;
    }

    private async void dgMaquinas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is Maquina maquinaSeleccionada)
        {
            this.MaquinaSeleccionada = maquinaSeleccionada;
            CerrarVentana();
        }
            
    }

    private void CerrarVentana()
    {
        // Remueve temporalmente el evento Closing
        Closing -= MaquinasConOTPendientesWindow_Closing;

        // Cierra la ventana
        Close();
    }

    private void MaquinasConOTPendientesWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
    }
}

