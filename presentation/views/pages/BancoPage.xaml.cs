using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Microsoft.Extensions.DependencyInjection;

namespace Gestion.presentation.views.pages
{
    public partial class BancoPage : Page
    {
        private readonly BancoViewModel _viewModel;
        public BancoPage(BancoViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Title = $"Bancos";

            Loaded += BancoPage_Loaded;
            dgBancos.ItemContainerGenerator.StatusChanged += DgBancos_StatusChanged;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            var ventana = App.ServiceProvider.GetRequiredService<AgregarBancoWindow>();
            ventana.Owner = Window.GetWindow(this);
            ventana.ShowDialog();

            string codigo = ventana.Codigo;
            string nombre = ventana.Nombre;
            string direccion = ventana.Direccion;
        }

        private async Task BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgBancos.SelectedItem is Banco bancoSeleccionado)
            {
                await _viewModel.delete(bancoSeleccionado.Id);
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgBancos.SelectedItem is Banco bancoSeleccionado)
            {
                var ventana = new EntidadEditorWindow(bancoSeleccionado)
                {
                    Title = "Editar Banco"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(dgBancos);
                    //var bancoEditado = (Banco)ventana.EntidadEditada;
                    //await _viewModel.updateBanco(bancoEditado);
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

        private async void dgBancos_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgBancos.SelectedItem is Banco bancoSeleccionado)
            {
                var ventana = new EntidadEditorWindow(bancoSeleccionado)
                {
                    Title = "Editar Banco"
                };

                if (ventana.ShowDialog() == true)
                {
                    var bancoEditado = (Banco)ventana.EntidadEditada;
                    await _viewModel.update(bancoEditado);
                }
            }
        }

        private async void dgBancos_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void DgBancos_StatusChanged(object? sender, EventArgs e)
        {
            GridFocus(dgBancos);
        }

        private async void BancoPage_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.LoadBancos();
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgBancos_StatusChanged;
                }
            }
        }
    }
}
