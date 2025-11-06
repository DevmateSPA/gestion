using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class ProveedorViewModel
{
    private readonly IProveedorService _proveedorService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Proveedor> Proveedores { get; set; } = new ObservableCollection<Proveedor>();
    public ProveedorViewModel(IProveedorService proveedorService, IDialogService dialogService)
    {
        _proveedorService = proveedorService;
        _dialogService = dialogService;
    }

    public async Task updateProveedor(Proveedor proveedor)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            await _proveedorService.Update(proveedor);

            var index = Proveedores.IndexOf(Proveedores.FirstOrDefault(b => b.Id == proveedor.Id));
            if (index >= 0)
                Proveedores[index] = proveedor;


        }, _dialogService, "Error al actualizar el proveedor");
    }


    public async Task LoadProveedores()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _proveedorService.FindAll();
            Proveedores.Clear();
            foreach (var proveedor in lista)
                Proveedores.Add(proveedor);
        }, _dialogService, "Error al cargar los proveedores");
    }
}
