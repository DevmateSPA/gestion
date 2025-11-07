using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages
{
    public partial class OperarioPage : Page
    {
        private readonly OperarioViewModel _viewModel;
        public OperarioPage(OperarioViewModel operarioViewModel)
        {
            InitializeComponent();
            _viewModel = operarioViewModel;
            DataContext = _viewModel;
            Title = $"Grupos";

            Loaded += OperarioPage_Loaded;
            dgOperarios.ItemContainerGenerator.StatusChanged += DgOperarios_StatusChanged;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Agregar operario...");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar operario...");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgOperarios.SelectedItem is Operario operarioSeleccionado)
            {
                var ventana = new EntidadEditorWindow(operarioSeleccionado)
                {
                    Title = "Editar Operario"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(dgOperarios);
                    //var operarioEditado = (Operario)ventana.EntidadEditada;
                    //await _viewModel.updateOperario(operarioEditado);
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

        private async void dgOperarios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgOperarios.SelectedItem is Operario operadorSeleccionada)
            {
                var ventana = new EntidadEditorWindow(operadorSeleccionada)
                {
                    Title = "Editar Operario",
                };

                if (ventana.ShowDialog() == true)
                {
                    var operarioEditado = (Operario)ventana.EntidadEditada;
                    await _viewModel.Update(operarioEditado);
                }
            }
        }
        private async void dgOperarios_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void DgOperarios_StatusChanged(object? sender, EventArgs e)
        {
            GridFocus(dgOperarios);
        }


        private async void OperarioPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadAll();
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgOperarios_StatusChanged;
                }
            }
        }
    }
}
