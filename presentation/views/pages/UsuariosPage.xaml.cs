using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.helpers;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;

namespace Gestion.presentation.views.pages
{
    public partial class UsuariosPage : Page
    {
        private DataGrid _dataGrid;
        private readonly UsuarioViewModel _viewModel;
        public UsuariosPage(UsuarioViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            Title = $"Usuarios";

            Loaded += async (_, _) =>
            {
                await _viewModel.LoadAllByEmpresa();
            };


            _dataGrid = dgUsuarios;

            txtBuscar.KeyDown += TxtBuscar_KeyDown;

        }

       private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            await new EditorEntidadBuilder<Usuario>()
                .Owner(Window.GetWindow(this)!)
                .Entidad(new Usuario())
                .Titulo("Agregar Usuario")
                .Guardar(_viewModel.Save)
                .Abrir();
        }

        private async Task Editar(Usuario psuario)
        {
            await new EditorEntidadBuilder<Usuario>()
                .Owner(Window.GetWindow(this)!)
                .Entidad(psuario)
                .Titulo("Editar Usuario")
                .Guardar(_viewModel.Update)
                .Abrir();
        }

        private async Task EditarSeleccionado()
        {
            if (dgUsuarios.SelectedItem is Usuario psuarioSeleccionado)
                await Editar(psuarioSeleccionado);
        }

        private async void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            await EditarSeleccionado();
        }

        private async void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            await EditarSeleccionado();
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var usuario = _dataGrid.SelectedItem as Usuario;  
            if (usuario == null)
            {
                MessageBox.Show("Seleccione alguna fila");
                return;
            }
            if (((Usuario)_dataGrid.SelectedItem).Nombre.Equals("SYS"))
            {
                return;
            }
            await EditorHelper.BorrarSeleccionado(
                seleccionado: _dataGrid.SelectedItem as Usuario,
                borrarAccion: async b => await _viewModel.Delete(b.Id),
                mensajeConfirmacion: $"¿Seguro que deseas eliminar el Usuario \"{((_dataGrid.SelectedItem as Usuario)?.Id)}\"?",
                mensajeExito: "Usuario eliminado correctamente.");
        }
    

     private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string? filtro = txtBuscar.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                // Si hay texto, cargar TODO antes de filtrar
                _viewModel.PageSize = 0;
                await _viewModel.LoadAllByEmpresa();
            }

            if (filtro == null)
                return;

            _viewModel.Buscar(filtro);
        }


        private async void dgUsuarios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_dataGrid.SelectedItem is Usuario psuarioSeleccionado)
            {
                var ventana = new EntidadEditorWindow(psuarioSeleccionado)
                {
                    Title = "Editar Usuario",
                };

                if (ventana.ShowDialog() == true)
                {
                    var psuarioEditado = (Usuario)ventana.EntidadEditada;
                    await _viewModel.Update(psuarioEditado);
                }
            }
        }

        private void dgUsuarios_PreviewKeyDown(object sender, KeyEventArgs e)
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
        }

        private void DgUsuarioes_StatusChanged(object? sender, EventArgs e)
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgUsuarioes_StatusChanged;
                }
            }
        }
    

        private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnBuscar_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}

