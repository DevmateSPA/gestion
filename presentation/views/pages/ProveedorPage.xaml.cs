using Gestion.core.model;
using Gestion.helpers;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.util;
using Gestion.presentation.views.windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gestion.presentation.views.pages
{
    public partial class ProveedorPage : Page
    {
        private DataGrid _dataGrid;

        private readonly ProveedorViewModel _viewModel;
        public ProveedorPage(ProveedorViewModel proveedorViewModel)
        {
            InitializeComponent();
            _viewModel = proveedorViewModel;
            DataContext = _viewModel;
            Title = $"Proveedores";

            Loaded += async (_, _) =>
            {
                await _viewModel.LoadPageByEmpresa(1);
                paginacion.SetTotalPages(_viewModel.TotalRegistros);
            };

            paginacion.PageChanged += async (nuevaPagina) =>
            {
                await _viewModel.LoadPageByEmpresa(nuevaPagina);
                paginacion.SetTotalPages(_viewModel.TotalRegistros);
            };

            paginacion.PageSizeChanged += async (size) =>
            {
                _viewModel.PageSize = size;

                if (size == 0)
                    await _viewModel.LoadAllByEmpresa(); // sin paginar
                else
                    await _viewModel.LoadPageByEmpresa(1); // resetear a página 1

                paginacion.SetTotalPages(_viewModel.TotalRegistros);
            };
            _dataGrid = dgProveedores;
            _dataGrid.ItemContainerGenerator.StatusChanged += DgProveedores_StatusChanged;

            txtBuscar.KeyDown += TxtBuscar_KeyDown;
        }

        private async void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            await new EditorEntidadBuilder<Proveedor>()
                .Owner(Window.GetWindow(this)!)
                .Entidad(new Proveedor())
                .Titulo("Agregar Proveedor")
                .Guardar(_viewModel.Save)
                .Abrir();
        }

        private async Task Editar(Proveedor proveedor)
        {
            await new EditorEntidadBuilder<Proveedor>()
                .Owner(Window.GetWindow(this)!)
                .Entidad(proveedor)
                .Titulo("Editar Proveedor")
                .Guardar(_viewModel.Update)
                .Abrir();
        }

        private async Task EditarSeleccionado()
        {
            if (dgProveedores.SelectedItem is Proveedor proveedorSeleccionado)
                await Editar(proveedorSeleccionado);
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
            await EditorHelper.BorrarSeleccionado(
                seleccionado: _dataGrid.SelectedItem as Proveedor,
                borrarAccion: async b => await _viewModel.Delete(b.Id),
                mensajeConfirmacion: $"¿Seguro que deseas eliminar el Proveedor \"{((_dataGrid.SelectedItem as Proveedor)?.Rut)}\"?",
                mensajeExito: "Proveedor eliminado correctamente.");
        }
        private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string? filtro = txtBuscar.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                // Si hay texto, cargar TODO antes de filtrar
                _viewModel.PageSize = 0;
                await _viewModel.LoadAllByEmpresa();

                paginacion.SetTotalPages(_viewModel.TotalRegistros);
            }
            else
            {
                // Si está vacío, volver a paginación normal
                if (_viewModel.PageSize == 0)
                {
                    _viewModel.PageSize = paginacion.CurrentPageSize; // el valor del TextBox de paginación
                }

                await _viewModel.LoadPageByEmpresa(1);
                paginacion.SetTotalPages(_viewModel.TotalRegistros);
            }

            if (filtro == null)
                return;

            _viewModel.Buscar(filtro);
        }

        private void BtnImprimir_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Imprimir listado...");
        }

        private async void dgProveedores_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_dataGrid.SelectedItem is Proveedor proveedorSeleccionado)
            {
                var ventana = new EntidadEditorWindow(proveedorSeleccionado)
                {
                    Title = "Editar Proveedor",
                };

                if (ventana.ShowDialog() == true)
                {
                    var proveedorEditado = (Proveedor)ventana.EntidadEditada;
                    await _viewModel.Update(proveedorEditado);
                }
            }
        }

        private void dgProveedores_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void DgProveedores_StatusChanged(object? sender, EventArgs e)
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgProveedores_StatusChanged;
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
