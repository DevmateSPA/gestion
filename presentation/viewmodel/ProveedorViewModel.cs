using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class ProveedorViewModel
{
    private readonly IProveedorService _proveedorService;
    public ObservableCollection<Proveedor> Proveedores { get; set; } = new ObservableCollection<Proveedor>();
    public ProveedorViewModel(IProveedorService proveedorService)
    {
        _proveedorService = proveedorService;
    }
    
    public async Task LoadProveedores()
    {
        var lista = await _proveedorService.FindAll();
        Proveedores.Clear();
        foreach (var proveedor in lista)
            Proveedores.Add(proveedor);
    }
}
