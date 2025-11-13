using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;

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
            await _viewModel.LoadAll();          // carga facturas
            await _viewModel.LoadAllDetalles();  // carga todos los detalles
        };

        _dataGrid = dgFacturas;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgFacturas_StatusChanged;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ventana = new EntidadEditorTableWindow(this, new Factura(), new List<Detalle>(), "Ingresar Factura");

        if (ventana.ShowDialog() == true)
        {
            var facturaEditado = (Factura)ventana.EntidadEditada;
            await _viewModel.Save(facturaEditado);
        }
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is Factura facturaSeleccionado)
            editar(facturaSeleccionado, "Editar Facturas");
    }

    private async void dgFacturas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is Factura facturaSeleccionado)
            editar(facturaSeleccionado, "Editar Facturas");
    }

    private async void editar(Factura factura, string titulo)
    {
        if (factura == null)
            return;

        var detallesFiltrados = _viewModel.Detalles
            .Where(d => d.Folio == factura.Folio)
            .ToList();

        factura.DetalleFactura.Clear();
        foreach (var detalle in detallesFiltrados)
            factura.DetalleFactura.Add(detalle);

        var detalleEditar = factura.DetalleFactura;

        var ventana = new EntidadEditorTableWindow(this, factura, detalleEditar, titulo);

        if (ventana.ShowDialog() == true)
        {
            var facturaEditada = (Factura)ventana.EntidadEditada;
            facturaEditada.DetalleFactura = detalleEditar;

            var folio = facturaEditada.Folio;

            // 🔹 Asignar Folio a todos los detalles editados
            foreach (var detalle in detalleEditar)
                detalle.Folio = folio;

            // 🔹 Separar detalles nuevos y existentes
            var nuevosDetalles = detalleEditar.Where(d => d.Id == 0).ToList();
            var detallesExistentes = detalleEditar.Where(d => d.Id != 0).ToList();

            // 🔹 Actualizar la colección global
            // Eliminar antiguos detalles del folio
            var detallesAntiguos = _viewModel.Detalles.Where(d => d.Folio == folio).ToList();
            foreach (var detalle in detallesAntiguos)
                _viewModel.Detalles.Remove(detalle);

            // Agregar todos los detalles editados (para que el ObservableCollection se actualice en UI)
            foreach (var detalle in detalleEditar)
                _viewModel.Detalles.Add(detalle);

            // 🔹 Guardar nuevos detalles
            if (nuevosDetalles.Any())
                await _viewModel.saveDetails(nuevosDetalles);

            // 🔹 Actualizar detalles existentes
            if (detallesExistentes.Any())
                await _viewModel.updateDetails(detallesExistentes);

            // 🔹 Actualizar la factura
            await _viewModel.Update(facturaEditada);
        }
        else
        {
            var facturaEditada = (Factura)ventana.EntidadEditada;
            facturaEditada.DetalleFactura = factura.DetalleFactura;
        }
    }

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
        MessageBox.Show($"Buscar: {txtBuscar.Text}");
    }

    private void BtnImprimir_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Imprimir listado...");
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
}
