using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class FacturaCompraViewModel : EntidadViewModel<FacturaCompra>
{
    private readonly IFacturaCompraProductoService _detalleService;
    private readonly IFacturaCompraService _facturaCompraService;
    public FacturaCompraViewModel(IFacturaCompraService facturaCompraService, IDialogService dialogService, IFacturaCompraProductoService detalleService)
        : base(facturaCompraService, dialogService)
    {
        _facturaCompraService = facturaCompraService;
        _detalleService = detalleService;
    }

    public virtual async Task<List<FacturaCompraProducto>> LoadDetailsByFolio(string folio)
    {
        List<FacturaCompraProducto>? detalles = null;

        await SafeExecutor.RunAsync(
            action: async () => detalles = await _detalleService.FindByFolio(folio),
            dialogService: _dialogService,
            mensajeError: $"Error al cargar los detalles de la factura con folio: {folio}");
            
        return detalles ?? [];
    }

    public override async Task Delete(long id)
    {
        FacturaCompra? factura = await _facturaCompraService.FindById(id);

        if (factura == null)
            return;

        await RunServiceAction(
        serviceAction: async () =>
        {
            bool eliminoFactura = await _facturaCompraService.DeleteById(id);
            bool detalles = false;

            if (eliminoFactura)
                detalles = await _detalleService.DeleteByFolio(factura.Folio);

            return eliminoFactura;
        },
        onSuccess: () => RemoveEntityById(id),
        mensajeError: "Error al eliminar la factura de compra");
    }

    public async Task SincronizarDetalles(
        IList<FacturaCompraProducto> originales,
        IList<FacturaCompraProducto> editados,
        FacturaCompra factura)
    {
        List<long> paraEliminar = originales.Any() ? 
            [.. originales.Where(o => !editados.Any(e => e.Id == o.Id)).Select(o => o.Id)]
            : [];

        List<FacturaCompraProducto> paraAgregar = [.. editados.Where(e => e.Id == 0)];

        List<FacturaCompraProducto> paraActualizar = [.. editados.Where(e => e.Id != 0 && originales.Any(o => o.Id == e.Id))];

        if (paraAgregar.Count != 0 || paraActualizar.Count != 0 || paraEliminar.Count != 0)
        {
            await MakeCrud(AsignarInfo(factura.Folio, factura.Tipo, paraAgregar),
                AsignarInfo(factura.Folio, factura.Tipo, paraActualizar),
                paraEliminar);
        }
    }

    private async Task MakeCrud(List<FacturaCompraProducto> paraAgregar, 
        List<FacturaCompraProducto> paraActualizar, 
        List<long> paraEliminar)
    {
        if (paraEliminar.Count != 0)
        {
            await RunServiceAction(() => _detalleService.DeleteByIds(paraEliminar), null, $"Error al eliminar los detalles de la factura");
        }

        if (paraAgregar.Count != 0)
        {
            await RunServiceAction(() => _detalleService.SaveAll(paraAgregar), null, $"Error al guardar los detalles de la factura");
        }

        if (paraActualizar.Count != 0)
        {
            await RunServiceAction(() => _detalleService.UpdateAll(paraActualizar), null, $"Error al actualizar los detalles de la factura");
        }
    }

    private static List<FacturaCompraProducto> AsignarInfo(string folio, string tipo, IList<FacturaCompraProducto> detalles)
    {
        var lista = detalles.ToList();

        if (lista.Count == 0)
            return lista;

        foreach (var detalle in detalles)
        {
            detalle.Folio = folio;
            detalle.Tipo = tipo;
        }

        return lista;
    }
}
