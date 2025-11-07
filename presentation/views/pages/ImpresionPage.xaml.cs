using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages
{
    public partial class ImpresionPage : Page
    {
        private readonly ImpresionViewModel _viewModel;
        public ImpresionPage(ImpresionViewModel impresionViewModel)
        {
            InitializeComponent();
            _viewModel = impresionViewModel;
            DataContext = _viewModel;
            Title = $"Impresiones";

            Loaded += async (_, _) => await _viewModel.LoadAll();
            dgImpresion.ItemContainerGenerator.StatusChanged += DgImpresion_StatusChanged;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Agregar impresion...");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar impresion...");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgImpresion.SelectedItem is Impresion impresionSeleccionado)
            {
                var ventana = new EntidadEditorWindow(impresionSeleccionado)
                {
                    Title = "Editar Impresion"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(dgImpresion);
                    //var impresionEditado = (Impresion)ventana.EntidadEditada;
                    //await _viewModel.updateImpresion(impresionEditado);
                }
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

        private async void dgImpresion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgImpresion.SelectedItem is Impresion impresionSeleccionado)
            {
                var ventana = new EntidadEditorWindow(impresionSeleccionado)
                {
                    Title = "Editar Impresión"
                };

                if (ventana.ShowDialog() == true)
                {
                    var impresionEditada = (Impresion)ventana.EntidadEditada;
                    await _viewModel.Update(impresionEditada);
                }
            }
        }

       private void dgImpresion_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var teclas = new[] { Key.Enter, Key.Insert, Key.Delete, Key.F2, Key.F4 };
            if (teclas.Contains(e.Key))
            {
                e.Handled = true;
            }
            if (e.Key == Key.Enter) BtnEditar_Click(sender, e);
            else if (e.Key == Key.Insert) BtnAgregar_Click(sender, e);
            else if (e.Key == Key.Delete) BtnEliminar_Click(sender, e);
            else if (e.Key == Key.F2) BtnBuscar_Click(sender, e);
            else if (e.Key == Key.F4) BtnImprimir_Click(sender, e);
        }

        private void DgImpresion_StatusChanged(object? sender, EventArgs e)
        {
            GridFocus(dgImpresion);
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgImpresion_StatusChanged;
                }
            }
        }
    }
}
