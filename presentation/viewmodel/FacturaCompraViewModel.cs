using System.Diagnostics;
using Gestion.core.interfaces.model;
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

    public virtual async Task<List<FacturaCompraProducto>> LoadDetailsByFolio(string folio, long empresaId)
    {
        List<FacturaCompraProducto>? detalles = null;

        await SafeExecutor.RunAsync(
            action: async () => detalles = await _detalleService.FindByFolio(folio, empresaId),
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
                detalles = await _detalleService.DeleteByFolio(factura.Folio, factura.Empresa);

            return eliminoFactura;
        },
        onSuccess: () => RemoveEntityById(id),
        mensajeError: "Error al eliminar la factura de compra");
    }

    public async Task SincronizarDetalles(
        IEnumerable<FacturaCompraProducto> originales,
        IEnumerable<FacturaCompraProducto> editados,
        FacturaCompra facturaCompra)
    {
        Log("======================================");
        Log("[SincronizarDetalles] INICIO");
        Log($"FC: {facturaCompra.Folio} | Empresa: {facturaCompra.Empresa}");

        Log("Originales:");
        foreach (var o in originales)
            Log($"  O -> Id={o.Id}");

        Log("Editados:");
        foreach (var e in editados)
            Log($"  E -> Id={e.Id}");

        List<long> paraEliminar = originales.Any()
            ? [.. originales
                .Where(o => !editados.Any(e => e.Id == o.Id))
                .Select(o => o.Id)]
            : [];

        List<FacturaCompraProducto> paraAgregar =
            [.. editados.Where(e => e.Id == 0)];

        List<FacturaCompraProducto> paraActualizar =
        [
            .. editados.Where(e =>
            {
                var original = originales.FirstOrDefault(o => o.Id == e.Id);
                return original != null && !original.Equals(e);
            })
        ];

        Log("Resultado sincronización:");
        Log($" - Para eliminar: {(paraEliminar.Count != 0 ? string.Join(", ", paraEliminar) : "(ninguno)")}");
        Log($" - Para agregar: {(paraAgregar.Count != 0 ? string.Join(", ", paraAgregar.Select(a => a.Id)) : "(ninguno)")}");
        Log($" - Para actualizar: {(paraActualizar.Count != 0 ? string.Join(", ", paraActualizar.Select(a => a.Id)) : "(ninguno)")}");

        if (paraEliminar.Count == 0 && paraAgregar.Count == 0 && paraActualizar.Count == 0)
        {
            Log(
                $"[SincronizarDetalles] No hay cambios: {originales.Count()} detalles comparados, 0 diferencias detectadas"
            );
            Log("[SincronizarDetalles] FIN");
            Log("======================================");
            return;
        }

        Log("[SincronizarDetalles] Ejecutando CRUD...");

        await MakeCrud(
            AsignarInfo(facturaCompra.Folio, facturaCompra.Empresa, facturaCompra.Tipo, paraAgregar),
            AsignarInfo(facturaCompra.Folio, facturaCompra.Empresa, facturaCompra.Tipo, paraActualizar),
            paraEliminar,
            facturaCompra.Empresa
        );

        Log("[SincronizarDetalles] FIN");
        Log("======================================");
    }

    private async Task MakeCrud(List<FacturaCompraProducto> paraAgregar, 
        List<FacturaCompraProducto> paraActualizar, 
        List<long> paraEliminar,
        long empresaId)
    {
        Log($"[MakeCrud] Iniciando CRUD...");
        if (paraEliminar.Count != 0)
        {
            Log($" - Eliminando IDs: {string.Join(", ", paraEliminar)}");
            await RunServiceAction(() => _detalleService.DeleteByIds(paraEliminar, empresaId), null, $"Error al eliminar los detalles de la factura");
        }

        if (paraAgregar.Count != 0)
        {
            Log($" - Agregando IDs: {string.Join(", ", paraAgregar.Select(a => a.Id))}");
            await RunServiceAction(() => _detalleService.SaveAll(paraAgregar), null, $"Error al guardar los detalles de la factura");
        }

        if (paraActualizar.Count != 0)
        {
            Log($" - Actualizando IDs: {string.Join(", ", paraActualizar.Select(a => a.Id))}");
            await RunServiceAction(() => _detalleService.UpdateAll(paraActualizar, empresaId), null, $"Error al actualizar los detalles de la factura");
        }

        Log($"[MakeCrud] Operación finalizada.");
    }

    private static List<FacturaCompraProducto> AsignarInfo(
        string folio, 
        long empresaId, 
        string tipo, IList<FacturaCompraProducto> detalles)
    {
        var lista = detalles.ToList();

        if (lista.Count == 0)
            return lista;

        foreach (FacturaCompraProducto detalle in detalles)
        {
            detalle.Folio = folio;
            detalle.Tipo = tipo;
            detalle.Empresa = empresaId;
        }

        return lista;
    }
}
