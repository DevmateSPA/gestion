using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;

namespace Gestion.presentation.views.pages
{
    public partial class OperarioPage : Page
    {
        private DataGrid _dataGrid;

        private readonly OperarioViewModel _viewModel;
        public OperarioPage(OperarioViewModel operarioViewModel)
        {
            InitializeComponent();
            _viewModel = operarioViewModel;
            DataContext = _viewModel;
            Title = $"Grupos";

            Loaded += async (_, _) => await _viewModel.LoadAll();
            _dataGrid = dgOperarios;
            _dataGrid.ItemContainerGenerator.StatusChanged += DgOperarios_StatusChanged;
        }

        private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var ventana = new EntidadEditorWindow(this,new Banco(), "Ingresar Operario");

            if (ventana.ShowDialog() == true)
            {
                var editado = (Operario)ventana.EntidadEditada;
                await _viewModel.Save(editado);
            }
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (_dataGrid.SelectedItem is Operario seleccionado)
            {
                if (DialogUtils.Confirmar($"¿Seguro que deseas eliminar al operario \"{seleccionado.Nombre}\"?", "Confirmar eliminación"))
                {
                    await _viewModel.Delete(seleccionado.Id);
                    DialogUtils.MostrarInfo("Operario eliminado correctamente.", "Éxito");
                }
            }
            else
            {
                DialogUtils.MostrarAdvertencia("Selecciona un operario antes de eliminar.", "Aviso");
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (_dataGrid.SelectedItem is Operario operarioSeleccionado)
            {
                var ventana = new EntidadEditorWindow(this, operarioSeleccionado)
                {
                    Title = "Editar Operario"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(_dataGrid);
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
            if (_dataGrid.SelectedItem is Operario operadorSeleccionada)
            {
                var ventana = new EntidadEditorWindow(this, operadorSeleccionada)
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
        private void dgOperarios_PreviewKeyDown(object sender, KeyEventArgs e)
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgOperarios_StatusChanged;
                }
            }
        }
    }
}
