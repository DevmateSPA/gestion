using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class ProductoViewModel
{
    private readonly IProductoService _productoService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Producto> Productos { get; set; } = new ObservableCollection<Producto>();
    public ProductoViewModel(IProductoService productoService, IDialogService dialogService)
    {
        _productoService = productoService;
        _dialogService = dialogService;
    }

    public async Task update(Producto producto)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            if (await _productoService.Update(producto))
            {
                var existing = Productos.FirstOrDefault(p => p.Id == producto.Id);
                if (existing != null)
                {
                    var index = Productos.IndexOf(existing);
                    if (index >= 0)
                        Productos[index] = producto;
                }
            }
        }, _dialogService, "Error al actualizar el producto");
    }

    public async Task LoadProductos()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _productoService.FindAll();
            Productos.Clear();
            foreach (var producto in lista)
                Productos.Add(producto);
        }, _dialogService, "Error al cargar los productos");
    }
}
