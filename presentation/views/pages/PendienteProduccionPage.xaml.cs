using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.views.util;
using System.IO;
using System.Threading.Tasks;
using Gestion.core.model.detalles;
using System.Collections.ObjectModel;
using Gestion.helpers;
using System.Drawing.Printing;
using Gestion.core.services;

namespace Gestion.presentation.views.pages;
    public partial class PendienteProduccionPage : Page
    {
    private readonly OrdenTrabajoViewModel _viewModel;
    
    private DataGrid _dataGrid;
    private readonly DialogService _dialogService = new(); 

    
    public PendienteProduccionPage(OrdenTrabajoViewModel ordenTrabajoViewModel)
    {
        InitializeComponent();
        _viewModel = ordenTrabajoViewModel;
        DataContext = _viewModel;
        Title = $"Pendientes Producción";

        Loaded += async (_, _) =>
        {
            await _viewModel.LoadPageByEmpresaAndPendiente(1);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        paginacion.PageChanged += async (nuevaPagina) =>
        {
            await _viewModel.LoadPageByEmpresaAndPendiente(nuevaPagina);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };

        paginacion.PageSizeChanged += async (size) =>
        {
            _viewModel.PageSize = size;

            if (size == 0)
                await _viewModel.LoadAllByEmpresaAndPendiente(); // sin paginar
            else
                await _viewModel.LoadPageByEmpresaAndPendiente(1); // resetear a página 1

            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        };
        _dataGrid = dgOrdenesTrabajo;
        _dataGrid.ItemContainerGenerator.StatusChanged += DataGrid_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }
    
    private async void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is OrdenTrabajo ordenTrabajoSeleccionado)
            await Editar(ordenTrabajoSeleccionado);
    }

    private async Task Editar(OrdenTrabajo ordenTrabajo)
    {
        if (ordenTrabajo == null)
            return;

        ordenTrabajo.Detalles = new ObservableCollection<DetalleOrdenTrabajo>(
            await _viewModel.LoadDetailsByFolio(ordenTrabajo.Folio, ordenTrabajo.Empresa));

        await new EditorEntidadBuilder<OrdenTrabajo>()
            .Owner(Window.GetWindow(this)!)
            .Entidad(ordenTrabajo)
            .Titulo("Visualizar Orden de Trabajo")
            .SoloLecutra()
            .Imprimir(orden =>
                PrintHelper.PrevisualizarOrdenTrabajo(
                    Window.GetWindow(this)!,
                    orden))
            .ShouldBtnImpresion()
            .Entregar(async win =>
            {
                var window = new EntregarOTModal();
                window.Owner = Window.GetWindow(this)!;
                if (window.ShowDialog() == true)
                {
                    DateTime? fecha = window.FechaEntrega;

                    if (fecha != null)
                    {
                        ordenTrabajo.FechaEntrega = fecha.Value;
                        ordenTrabajo.OrdenEntregada = fecha.Value;
                        ordenTrabajo.FechaTermino = fecha.Value;
                        _viewModel.Update(ordenTrabajo);
                        _dialogService.ShowMessage("Orden marcada como entregada");
                        window.Close();
                        win.Close();
                        BtnBuscar_Click(null,null);
                    }
                }
            })
            .Abrir();
    }
    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        // 1️⃣ Leer impresora guardada
        string impresora = LocalConfig.ObtenerImpresora();

        // 2️⃣ Validar si existe o es válida
        bool impresoraValida =
            !string.IsNullOrWhiteSpace(impresora) &&
            impresora != "" &&
            PrinterSettings.InstalledPrinters.Cast<string>()
                .Any(p => p == impresora);

        // 3️⃣ Si no hay impresora válida, pedirla
        if (!impresoraValida)
        {
            var modal = new ImpresoraModal();

            if (modal.ShowDialog() != true)
                return; // usuario canceló

            impresora = modal.ImpresoraSeleccionada;

            if (string.IsNullOrWhiteSpace(impresora))
                return;
        }

        // 4️⃣ Imprimir directo
        string pdfPath =
            PrintUtils.GenerarListadoOTPendientes(_viewModel.Entidades.ToList());

        string sumatra = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "SumatraPDF.exe"
        );

        PrintUtils.PrintFile(pdfPath, impresora, sumatra);
    }
    private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        string? filtro = txtBuscar.Text?.Trim();

        if (!string.IsNullOrWhiteSpace(filtro))
        {
            // Si hay texto, cargar TODO antes de filtrar
            _viewModel.PageSize = 0;
            await _viewModel.LoadAllByEmpresaAndPendiente();

            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        }
        else
        {
            // Si está vacío, volver a paginación normal
            if (_viewModel.PageSize == 0)
            {
                _viewModel.PageSize = paginacion.CurrentPageSize; // el valor del TextBox de paginación
            }

            await _viewModel.LoadPageByEmpresaAndPendiente(1);
            paginacion.SetTotalPages(_viewModel.TotalRegistros);
        }

        if (filtro == null)
            return;

        _viewModel.Buscar(filtro);
    }

    private void DataGrid_StatusChanged(object? sender, EventArgs e)
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
                    firstRow.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                dataGrid.ItemContainerGenerator.StatusChanged -= DataGrid_StatusChanged;
            }
        }
    }

    private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var teclas = new[] { Key.Enter, Key.Insert, Key.Delete, Key.F2, Key.F4 };

        if (!teclas.Contains(e.Key))
            return;

        e.Handled = true;

        switch (e.Key)
        {
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