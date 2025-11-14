using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class FacturaViewModel : EntidadViewModel<Factura>
{
    private readonly IDetalleService _detalleService;
    public ObservableCollection<Factura> Facturas => Entidades;
    public ObservableCollection<Detalle> Detalles = new ObservableCollection<Detalle>();
    public FacturaViewModel(IFacturaService facturaService, IDetalleService detalleService, IDialogService dialogService)
        : base(facturaService, dialogService)
    {
        _detalleService = detalleService;
    }

    public async Task saveDetails(List<Detalle> detalles)
    {
        foreach (var detalle in detalles)
        {
            await SafeExecutor.RunAsync(async () =>
            {
                await _detalleService.Save(detalle);

            }, _dialogService, $"Error al guardar el producto {detalle.Producto}");
        }
    }

    public async Task updateDetails(List<Detalle> detalles)
    {
        foreach (var detalle in detalles)
        {
            await SafeExecutor.RunAsync(async () =>
            {
                await _detalleService.Update(detalle);

            }, _dialogService, $"Error al actualizar el producto {detalle.Producto}");
        }

        
    }
        
    public async Task LoadAllDetalles()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var detalles = await _detalleService.FindAll();

            Detalles.Clear();
            foreach (var d in detalles)
                Detalles.Add(d);

        }, _dialogService, "Error al cargar los detalles de facturas");
    }
}
