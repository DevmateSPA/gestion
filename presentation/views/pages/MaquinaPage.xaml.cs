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
    public partial class MaquinaPage : Page
    { 
        private DataGrid _dataGrid;

        private readonly MaquinaViewModel _viewModel;
        public MaquinaPage(MaquinaViewModel maquinaViewModel)
        {
            InitializeComponent();
            _viewModel = maquinaViewModel;
            DataContext = _viewModel;
            Title = $"Maquinas";

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
            _dataGrid = dgMaquinas;
            _dataGrid.ItemContainerGenerator.StatusChanged += DgMaquinas_StatusChanged;

            txtBuscar.KeyDown += TxtBuscar_KeyDown;
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            EditorHelper.Abrir(
                owner: Window.GetWindow(this),
                entidad: new Maquina(),
                accion: async entidad => await _viewModel.Save((Maquina)entidad),
                titulo: "Agregar Máquina");
        }

        private void Editar(Maquina entity)
        {
            EditorHelper.Abrir(
                owner: Window.GetWindow(this),
                entidad: entity,
                accion: async entidad => await _viewModel.Update((Maquina)entidad),
                titulo: "Editar Máquina");
        }

        private void EditarSeleccionado()
        {
            if (dgMaquinas.SelectedItem is Maquina seleccionado)
                Editar(seleccionado);
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            EditarSeleccionado();
        }
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditarSeleccionado();
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (_dataGrid.SelectedItem is Maquina seleccionado)
            {
                if (DialogUtils.Confirmar($"¿Seguro que deseas eliminar la maquina \"{seleccionado.Codigo}\"?", "Confirmar eliminación"))
                {
                    await _viewModel.Delete(seleccionado.Id);
                    DialogUtils.MostrarInfo("Maquina eliminada correctamente.", "Éxito");
                }
            }
            else
            {
                DialogUtils.MostrarAdvertencia("Selecciona una maquina antes de eliminar.", "Aviso");
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

        private void dgMaquinas_PreviewKeyDown(object sender, KeyEventArgs e)
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

                    dataGrid.ItemContainerGenerator.StatusChanged -= DgMaquinas_StatusChanged;
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
