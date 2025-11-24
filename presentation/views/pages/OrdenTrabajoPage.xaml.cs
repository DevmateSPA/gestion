using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;
using System.ComponentModel;

namespace Gestion.presentation.views.pages;

public partial class OrdenTrabajoPage : Page
{
    private DataGrid _dataGrid; 

    private readonly OrdenTrabajoViewModel _viewModel;
    public OrdenTrabajoPage(OrdenTrabajoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        Title = $"Ordenes de Trabajo";

        Loaded += async (_, _) =>
        {
            await _viewModel.LoadAll();  
        };
        _dataGrid = dgOrdenTrabajo;
        _dataGrid.ItemContainerGenerator.StatusChanged += DgOrdenTrabajo_StatusChanged;
    }

    private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
    {
        var ordenTrabajo = new OrdenTrabajo();
        var ventana = new EntidadEditorWindow(this, ordenTrabajo, "Ingresar Orden de Trabajo");

        if (ventana.ShowDialog() != true)
            return; 

        var ordenTrabajoEditado = (OrdenTrabajo)ventana.EntidadEditada;

        await _viewModel.Save(ordenTrabajoEditado);
    }

    private async void BtnEditar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is OrdenTrabajo ordenTrabajoSeleccionado)
            editar(ordenTrabajoSeleccionado, "Editar Orden de Trabajo");
    }

    private async void dgOrdenTrabajo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_dataGrid.SelectedItem is OrdenTrabajo ordenTrabajoSeleccionado)
            editar(ordenTrabajoSeleccionado, "Editar Orden de Trabajo");
    }

    private async void editar(OrdenTrabajo ordenTrabajo, string titulo)
    {
        if (ordenTrabajo == null)
            return;

        var ventana = new EntidadEditorWindow(this, ordenTrabajo, titulo);

        if (ventana.ShowDialog() != true)
        {
            var ordenTrabajoCancelada = (OrdenTrabajo)ventana.EntidadEditada;
            return;
        }

        var ordenTrabajoEditada = (OrdenTrabajo)ventana.EntidadEditada;

        await _viewModel.Update(ordenTrabajoEditada);
    }

    private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
    {
        if (_dataGrid.SelectedItem is OrdenTrabajo seleccionado)
        {
            if (DialogUtils.Confirmar($"¿Seguro que deseas eliminar la orden de trabajo \"{seleccionado.Folio}\"?", "Confirmar eliminación"))
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

    private void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        //MessageBox.Show($"Buscar: {txtBuscar.Text}");
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
}
