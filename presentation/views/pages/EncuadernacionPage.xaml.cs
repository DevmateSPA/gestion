using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages
{
    public partial class EncuadernacionPage : Page
    {
        private readonly EncuadernacionViewModel _viewModel;
        public EncuadernacionPage(EncuadernacionViewModel encuadernacionViewModel)
        {
            InitializeComponent();
            _viewModel = encuadernacionViewModel;
            DataContext = _viewModel;
            Title = $"Encuadernacion";

            Loaded += EncuadernacionPage_Loaded;
            dgEncuadernacion.ItemContainerGenerator.StatusChanged += DgEncuadernacion_StatusChanged;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Agregar encuadernacion...");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar encuadernacion...");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgEncuadernacion.SelectedItem is Encuadernacion encuadernacionSeleccionado)
            {
                var ventana = new EntidadEditorWindow(encuadernacionSeleccionado)
                {
                    Title = "Editar Encuadernacion"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(dgEncuadernacion);
                    //var encuadernacionEditado = (Encuadernacion)ventana.EntidadEditada;
                    //await _viewModel.updateEncuadernacion(encuadernacionEditado);
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

        private async void dgEncuadernacion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgEncuadernacion.SelectedItem is Encuadernacion encuadernacionSeleccionada)
            {
                var ventana = new EntidadEditorWindow(encuadernacionSeleccionada)
                {
                    Title = "Editar Encuadernacion"
                };

                if (ventana.ShowDialog() == true)
                {
                    var encuadernacionEditada = (Encuadernacion)ventana.EntidadEditada;
                    await _viewModel.update(encuadernacionEditada);
                }
            }
        }

        private async void dgEncuadernacion_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void DgEncuadernacion_StatusChanged(object? sender, EventArgs e)
        {
            GridFocus(dgEncuadernacion);
        }

        private async void EncuadernacionPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadEncuadernaciones();
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgEncuadernacion_StatusChanged;
                }
            }
        }
    }
}
