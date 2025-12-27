using Gestion.core.model;
using Gestion.core.session;
using Gestion.helpers;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.util;
using Gestion.presentation.views.windows;
using System.Printing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Gestion.presentation.views.pages;

public partial class FacturaPage : Page
{
    private DataGrid _dataGrid;

    private readonly FacturaViewModel _viewModel;
    public FacturaPage(FacturaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Facturas";

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

        _dataGrid = dgFacturas;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgFacturas_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }

    private void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        EditorHelper.Abrir(
            owner: Window.GetWindow(this),
            entidad: new Factura(),
            accion: async entidad => await _viewModel.Save((Factura)entidad),
            titulo: "Agregar Factura");
    }

    private void Editar(Factura entity)
    {
        EditorHelper.Abrir(
            owner: Window.GetWindow(this),
            entidad: entity,
            accion: async entidad => await _viewModel.Update((Factura)entidad),
            titulo: "Editar Factura");
    }

    private void EditarSeleccionado()
    {
        if (dgFacturas.SelectedItem is Factura seleccionado)
            Editar(seleccionado);
    }

    private void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        EditarSeleccionado();
    }
    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        EditarSeleccionado();
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        await EditorHelper.BorrarSeleccionado(
            seleccionado: _dataGrid.SelectedItem as Factura,
            borrarAccion: async b => await _viewModel.Delete(b.Id),
            mensajeConfirmacion: $"¿Seguro que deseas eliminar la Factura \"{((_dataGrid.SelectedItem as Factura)?.Folio)}\"?",
            mensajeExito: "Factura eliminada correctamente.");
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
        //ImprimirDirecto(_dataGrid,"Microsoft Print to PDF");
        //ImprimirDataGridCompleto(_dataGrid,"Microsoft Print to PDF");
    }

    private void DgFacturas_StatusChanged(object? sender, EventArgs e)
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

                dataGrid.ItemContainerGenerator.StatusChanged -= DgFacturas_StatusChanged;
            }
        }
    }

    private void dgFacturas_PreviewKeyDown(object sender, KeyEventArgs e)
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

        public void SeleccionarImpresora()
    {
        PrintDialog dlg = new PrintDialog();

        // mostrar ventana de impresoras
        bool? result = dlg.ShowDialog();

        if (result == true)
        {
            // Crear un documento simple (puede ser lo que quieras imprimir)
            FlowDocument doc = new FlowDocument(new Paragraph(new Run("Prueba de impresión")));
            doc.Name = "DocumentoPrueba";

            // Convertir a IDocumentPaginatorSource
            IDocumentPaginatorSource idp = doc;

            // Enviar a imprimir
            dlg.PrintDocument(idp.DocumentPaginator, "Impresión WPF");
        }
    }
    public void ImprimirDataGrid(DataGrid grid)
    {
        PrintDialog pd = new PrintDialog();

        if (pd.ShowDialog() == true)
        {
            // Ajustar tamaño del DataGrid a la página
            grid.Measure(new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight));
            grid.Arrange(new Rect(new Point(0, 0), grid.DesiredSize));

            pd.PrintVisual(grid, "Impresión de DataGrid");
        }
    }

    public void ImprimirDirecto(DataGrid grid, string nombreImpresora)
    {
        // Obtener impresora
        LocalPrintServer printServer = new LocalPrintServer();
        PrintQueue cola = printServer.GetPrintQueue(nombreImpresora);

        // Si no existe, mostrar mensaje
        if (cola == null)
        {
            MessageBox.Show($"La impresora '{nombreImpresora}' no existe.");
            return;
        }

        // Preparar PrintDialog usando esa impresora
        PrintDialog pd = new PrintDialog
        {
            PrintQueue = cola
        };

        // Ajustar el DataGrid
        grid.Measure(new Size(pd.PrintableAreaWidth, pd.PrintableAreaHeight));
        grid.Arrange(new Rect(new Point(0, 0), grid.DesiredSize));

        // Imprimir sin mostrar dialogo
        pd.PrintVisual(grid, "Impresión directa");
    }

    
}
