using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages
{
    public partial class GrupoPage : Page
    {
        private readonly GrupoViewModel _viewModel;
        public GrupoPage(GrupoViewModel grupoViewModel)
        {
            InitializeComponent();
            _viewModel = grupoViewModel;
            DataContext = _viewModel;
            Title = $"Grupos";

            Loaded += async (_, _) => await _viewModel.LoadAll();
            dgGrupos.ItemContainerGenerator.StatusChanged += DgGrupos_StatusChanged;
        }
        

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Agregar grupo...");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar grupo...");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgGrupos.SelectedItem is Grupo grupoSeleccionado)
            {
                var ventana = new EntidadEditorWindow(grupoSeleccionado)
                {
                    Title = "Editar Grupo"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(dgGrupos);
                    //var grupoEditado = (Grupo)ventana.EntidadEditada;
                    //await _viewModel.updateGrupo(grupoEditado);
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

        private async void dgGrupos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgGrupos.SelectedItem is Grupo grupoSeleccionado)
            {
                var ventana = new EntidadEditorWindow(grupoSeleccionado)
                {
                    Title = "Editar Grupo"
                };

                if (ventana.ShowDialog() == true)
                {
                    var grupoEditado = (Grupo)ventana.EntidadEditada;
                    await _viewModel.Update(grupoEditado);
                }
            }
        }
 
        private void dgGrupos_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void DgGrupos_StatusChanged(object? sender, EventArgs e)
        {
            GridFocus(dgGrupos);
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgGrupos_StatusChanged;
                }
            }
        }
    }
}
