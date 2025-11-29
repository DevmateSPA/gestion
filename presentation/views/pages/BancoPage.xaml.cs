using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Microsoft.Extensions.DependencyInjection;
using Gestion.presentation.utils;
using System.Drawing.Printing;
using System.Windows.Documents;
using System.Printing;

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

        Loaded += async (_, _) => await _viewModel.LoadAllByEmpresa();
        _dataGrid = dgBancos;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgBancos_StatusChanged;

        txtBuscar.KeyDown += TxtBuscar_KeyDown;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EntidadEditorWindow(this,new Banco(), "Ingresar Banco");

        if (ventana.ShowDialog() == true)
        {
            var bancoEditado = (Banco)ventana.EntidadEditada;
            await _viewModel.Save(bancoEditado);
        }
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (dgBancos.SelectedItem is Banco bancoSeleccionado)
            editar(bancoSeleccionado, "Editar Banco");
    }

    private async void dgBancos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dgBancos.SelectedItem is Banco bancoSeleccionado)
            editar(bancoSeleccionado, "Editar Banco");
    }

    private async void editar(Banco banco, string titulo)
    {
        var ventana = new EntidadEditorWindow(this,banco, titulo);

        if (ventana.ShowDialog() == true)
        {
            var bancoEditado = (Banco)ventana.EntidadEditada;
            await _viewModel.Update(bancoEditado);
        }
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is Banco seleccionado)
        {
            if (DialogUtils.Confirmar($"¿Seguro que deseas eliminar al banco \"{seleccionado.Nombre}\"?", "Confirmar eliminación"))
            {
                await _viewModel.Delete(seleccionado.Id);
                DialogUtils.MostrarInfo("Banco eliminado correctamente.", "Éxito");
            }
        }
        else
        {
            DialogUtils.MostrarAdvertencia("Selecciona un banco antes de eliminar.", "Aviso");
        }
    }

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.Filtro = txtBuscar.Text;
        _viewModel.Buscar();    
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

    private void dgBancos_PreviewKeyDown(object sender, KeyEventArgs e)
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
