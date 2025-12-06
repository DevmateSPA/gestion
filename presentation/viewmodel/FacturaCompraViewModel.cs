using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class FacturaCompraViewModel : EntidadViewModel<FacturaCompra>
{
    private readonly IFacturaCompraProductoService _detalleService;
    public FacturaCompraViewModel(IFacturaCompraService facturaCompraService, IDialogService dialogService, IFacturaCompraProductoService detalleService)
        : base(facturaCompraService, dialogService)
    {
        _detalleService = detalleService;
    }

    public async Task SincronizarDetalles(
        IList<FacturaCompraProducto> originales,
        IList<FacturaCompraProducto> editados,
        FacturaCompra factura)
    {
        List<FacturaCompraProducto> paraEliminar = originales.Any() ?
        [.. originales.Where(o => !editados.Any(e => e.Id == o.Id))] : [];

        foreach (var del in paraEliminar.Where(d => d.Id != 0))
            await _detalleService.DeleteById(del.Id);

        foreach (var detalle in editados)
        {
            detalle.Folio = factura.Folio;
            detalle.Tipo = factura.Tipo;

            if (detalle.Id == 0)
                await _detalleService.Save(detalle);
            else
                await _detalleService.Update(detalle);
        }
    }
}
