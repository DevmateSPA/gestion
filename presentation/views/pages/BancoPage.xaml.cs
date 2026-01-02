using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.util;
using System.Printing;
using Gestion.helpers;
using System.Threading.Tasks;

namespace Gestion.presentation.views.pages;

public partial class BancoPage : Page
{
    private DataGrid _dataGrid;

    private readonly BancoViewModel _viewModel;
    public BancoPage(BancoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Bancos";

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

        _dataGrid = dgBancos;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgBancos_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        await new EditorEntidadBuilder<Banco>()
            .Owner(Window.GetWindow(this)!)
            .Entidad(new Banco())
            .Titulo("Agregar Banco")
            .Guardar(_viewModel.Save)
            .Abrir();
    }

    private async Task Editar(Banco banco)
    {
        await new EditorEntidadBuilder<Banco>()
            .Owner(Window.GetWindow(this)!)
            .Entidad(banco)
            .Titulo("Editar Banco")
            .Guardar(_viewModel.Update)
            .Abrir();
    }

    private async Task EditarSeleccionado()
    {
        if (dgBancos.SelectedItem is Banco bancoSeleccionado)
            await Editar(bancoSeleccionado);
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        await EditarSeleccionado();
    }

    private async void DgBancos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        await EditarSeleccionado();
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        await EditorHelper.BorrarSeleccionado(
            seleccionado: _dataGrid.SelectedItem as Banco,
            borrarAccion: async b => await _viewModel.Delete(b.Id),
            mensajeConfirmacion: $"¿Seguro que deseas eliminar al banco \"{((_dataGrid.SelectedItem as Banco)?.Nombre)}\"?",
            mensajeExito: "Banco eliminado correctamente.");
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
        //SeleccionarImpresora();
        //ImprimirDataGrid(dgBancos);
        //ImprimirDirecto(dgBancos,"Microsoft Print to PDF");
        ImprimirDataGridCompleto(dgBancos,"Microsoft Print to PDF");
    }

    private void DgBancos_StatusChanged(object? sender, EventArgs e)
    {
        GridFocus(dgBancos);
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

                dataGrid.ItemContainerGenerator.StatusChanged -= DgBancos_StatusChanged;
            }
        }
    }

    private void DgBancos_PreviewKeyDown(object sender, KeyEventArgs e)
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

    public void ImprimirDataGridCompleto(DataGrid grid, string impresora)
{
    // Obtener impresora
    LocalPrintServer server = new LocalPrintServer();
    PrintQueue cola = server.GetPrintQueue(impresora);

    if (cola == null)
    {
        MessageBox.Show($"La impresora '{impresora}' no existe.");
        return;
    }

    PrintDialog pd = new PrintDialog
    {
        PrintQueue = cola
    };

    // 1. Guardar tamaño original
    double originalHeight = grid.Height;
    double originalWidth = grid.Width;

    // 2. Expandir para mostrar todo
    grid.Height = double.NaN; // Auto
    grid.Width = pd.PrintableAreaWidth;

    grid.UpdateLayout();

    // Medir nuevamente
    grid.Measure(new Size(pd.PrintableAreaWidth, double.PositiveInfinity));
    grid.Arrange(new Rect(new Point(0, 0), grid.DesiredSize));

    // 3. Imprimir TODO el contenido
    pd.PrintVisual(grid, "Impresion DataGrid Completo");

    // 4. Restaurar tamaños
    grid.Height = originalHeight;
    grid.Width = originalWidth;
    grid.UpdateLayout();
}
}
