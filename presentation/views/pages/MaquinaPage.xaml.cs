using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages
{
    public partial class MaquinaPage : Page
    {
        private readonly MaquinaViewModel _viewModel;
        public MaquinaPage(MaquinaViewModel maquinaViewModel)
        {
            InitializeComponent();
            _viewModel = maquinaViewModel;
            DataContext = _viewModel;
            Title = $"Grupos";

            Loaded += MaquinaPage_Loaded;
            dgMaquinas.ItemContainerGenerator.StatusChanged += DgMaquinas_StatusChanged;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Agregar maquina...");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar maquina...");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgMaquinas.SelectedItem is Maquina maquinaSeleccionado)
            {
                var ventana = new EntidadEditorWindow(maquinaSeleccionado)
                {
                    Title = "Editar Maquina"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(dgMaquinas);
                    //var maquinaEditado = (Maquina)ventana.EntidadEditada;
                    //await _viewModel.updateMaquina(maquinaEditado);
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

        private async void dgMaquinas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgMaquinas.SelectedItem is Maquina maquinaSeleccionada)
            {
                var ventana = new EntidadEditorWindow(maquinaSeleccionada)
                {
                    Title = "Editar Máquina",
                };

                if (ventana.ShowDialog() == true)
                {
                    var maquinaEditada = (Maquina)ventana.EntidadEditada;
                    await _viewModel.Update(maquinaEditada);
                }
            }
        }

        private async void dgMaquinas_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void DgMaquinas_StatusChanged(object? sender, EventArgs e)
        {
            GridFocus(dgMaquinas);
        }


        private async void MaquinaPage_Loaded(object sender, RoutedEventArgs e)
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgMaquinas_StatusChanged;
                }
            }
        }
    }
}
