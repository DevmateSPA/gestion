using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class ProductoViewModel
{
    private readonly IProductoService _productoService;
    public ObservableCollection<Producto> Productos { get; set; } = new ObservableCollection<Producto>();
    public ProductoViewModel(IProductoService productoService)
    {
        _productoService = productoService;
    }

    public async Task LoadProductos()
    {
        var lista = await _productoService.FindAll();
        Productos.Clear();
        foreach (var producto in lista)
            Productos.Add(producto);
    }
}
