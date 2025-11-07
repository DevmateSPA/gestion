using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages
{
    public partial class FotomecanicaPage : Page
    {
        private readonly FotomecanicaViewModel _viewModel;
        public FotomecanicaPage(FotomecanicaViewModel fotomecanicaViewModel)
        {
            InitializeComponent();
            _viewModel = fotomecanicaViewModel;
            DataContext = _viewModel;
            Title = $"Encuadernacion";

            Loaded += FotomecanicaPage_Loaded;
            dgFotomecanica.ItemContainerGenerator.StatusChanged += DgFotomecanica_StatusChanged;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Agregar fotomecanica...");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar fotomecanica...");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgFotomecanica.SelectedItem is Fotomecanica fotomecanicaSeleccionado)
            {
                var ventana = new EntidadEditorWindow(fotomecanicaSeleccionado)
                {
                    Title = "Editar Fotomecanica"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(dgFotomecanica);
                    //var fotomecanicaEditado = (Fotomecanica)ventana.EntidadEditada;
                    //await _viewModel.updateFotomecanica(fotomecanicaEditado);
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

        private async void dgFotomecanica_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgFotomecanica.SelectedItem is Fotomecanica fotomecanicaSeleccionada)
            {
                var ventana = new EntidadEditorWindow(fotomecanicaSeleccionada)
                {
                    Title = "Editar Fotomecánica",
                };

                if (ventana.ShowDialog() == true)
                {
                    var fotomecanicaEditada = (Fotomecanica)ventana.EntidadEditada;
                    await _viewModel.Update(fotomecanicaEditada);
                }
            }
        }

        private void dgFotomecanica_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void DgFotomecanica_StatusChanged(object? sender, EventArgs e)
        {
            GridFocus(dgFotomecanica);
        }


        private async void FotomecanicaPage_Loaded(object sender, RoutedEventArgs e)
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgFotomecanica_StatusChanged;
                }
            }
        }
    }
}
