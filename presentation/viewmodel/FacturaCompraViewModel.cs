using System.Collections.ObjectModel;
using System.Reflection;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;
using Gestion.presentation.utils;

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
            var dateProp = PageUtils.GetDateProperty(typeof(FacturaCompra));
            if (dateProp != null)
            {
                lista = lista.OrderByDescending(x => dateProp.GetValue(x)).ToList();
            }
            FacturasCompra.Clear();
            foreach (var entidad in lista)
                addEntity(entidad);
        }, _dialogService, $"Error al cargar las facturas");
    }

}
