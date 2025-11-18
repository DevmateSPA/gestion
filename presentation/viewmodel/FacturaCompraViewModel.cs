using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class FacturaCompraViewModel : EntidadViewModel<FacturaCompra>
{
    private readonly IFacturaCompraService _facturaService;
    public ObservableCollection<FacturaCompra> FacturasCompra => Entidades;
    public FacturaCompraViewModel(IFacturaCompraService facturaCompraService, IDialogService dialogService)
        : base(facturaCompraService, dialogService)
    {
        _facturaService = facturaCompraService;
    }

    public override async Task LoadAll()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _facturaService.FindAllWithDetails();
            FacturasCompra.Clear();
            foreach (var entidad in lista)
                addEntity(entidad);
        }, _dialogService, $"Error al cargar las facturas");
    }
}
