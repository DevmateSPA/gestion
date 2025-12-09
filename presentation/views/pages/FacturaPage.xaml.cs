using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;
using Gestion.core.session;
using System.Windows.Documents;
using System.Printing;

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
            await _viewModel.LoadAllByEmpresa();
        };

        _dataGrid = dgFacturas;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgFacturas_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var factura = new Factura();
        factura.Empresa = SesionApp.IdEmpresa;
        var ventana = new EntidadEditorWindow(this, factura, "Ingresar Factura");

        if (ventana.ShowDialog() != true)
            return; 

        var facturaEditado = (Factura)ventana.EntidadEditada;

        await _viewModel.Save(facturaEditado);
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is Factura facturaSeleccionado)
            await editar(facturaSeleccionado, "Editar Facturas");
    }

    private async void dgFacturas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is Factura facturaSeleccionado)
            await editar(facturaSeleccionado, "Editar Facturas");
    }

    // Metodo Editar -------------------------------------------- |
    private async Task editar(Factura factura, string titulo)
    {
        if (factura == null)
            return;

        var ventana = new EntidadEditorWindow(this, factura, titulo);

        if (ventana.ShowDialog() != true)
        {
            var facturaCancelada = (Factura)ventana.EntidadEditada;
            return;
        }

        var facturaEditada = (Factura)ventana.EntidadEditada;

        await _viewModel.Update(facturaEditada);
    }

    // ------------------------------------------------------ |

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is Factura seleccionado)
        {
            if (DialogUtils.Confirmar($"¿Seguro que deseas eliminar la factura \"{seleccionado.Id}\"?", "Confirmar eliminación"))
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
    

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Buscar(txtBuscar.Text);    
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
