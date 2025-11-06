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

            Loaded += ImpresionPage_Loaded;
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
                    await _viewModel.updateImpresion(impresionEditada);
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

        private async void ImpresionPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadImpresion();
        }
    }
}
