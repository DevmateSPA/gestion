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

            Loaded += GrupoPage_Loaded;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Agregar banco...");
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Eliminar banco...");
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Editar banco...");
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
                    await _viewModel.updateGrupo(grupoEditado);
                }
            }
        }
 
        // Atajos de teclado
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Insert) BtnAgregar_Click(sender, e);
            else if (e.Key == Key.Delete) BtnEliminar_Click(sender, e);
            else if (e.Key == Key.Enter) BtnEditar_Click(sender, e);
            else if (e.Key == Key.F2) BtnBuscar_Click(sender, e);
            else if (e.Key == Key.F4) BtnImprimir_Click(sender, e);
        }

        private async void GrupoPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadGrupos();
        }
    }
}
