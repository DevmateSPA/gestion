using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.views.util;

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
             var ventana = new EntidadEditorWindow(new Proveedor(), "Ingresar Proveedor");

            if (ventana.ShowDialog() == true)
            {
                var editado = (Proveedor)ventana.EntidadEditada;
                await _viewModel.Save(editado);
            }
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (_dataGrid.SelectedItem is Proveedor seleccionado)
            {
                if (DialogUtils.Confirmar($"¿Seguro que deseas eliminar el proveedor \"{seleccionado.Razon_Social}\"?", "Confirmar eliminación"))
                {
                    await _viewModel.Delete(seleccionado.Id);
                    DialogUtils.MostrarInfo("Proveedor eliminado correctamente.", "Éxito");
                }
            }
            else
            {
                DialogUtils.MostrarAdvertencia("Selecciona un proveedor antes de eliminar.", "Aviso");
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (_dataGrid.SelectedItem is Proveedor proveedorSeleccionado)
            {
                var ventana = new EntidadEditorWindow(proveedorSeleccionado)
                {
                    Title = "Editar Proveedor"
                };

                if (ventana.ShowDialog() == true)
                {
                    GridFocus(_dataGrid);
                    //var proveedorEditado = (Proveedore)ventana.EntidadEditada;
                    //await _viewModel.updateProveedore(proveedorEditado);
                }
            }
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
