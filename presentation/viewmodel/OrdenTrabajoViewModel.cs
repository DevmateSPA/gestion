using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.model.detalles;

namespace Gestion.presentation.viewmodel; 

public class OrdenTrabajoViewModel : EntidadViewModel<OrdenTrabajo>, INotifyPropertyChanged
{
    private readonly IOrdenTrabajoService _ordenTrabajoService;
    private readonly IDetalleOTService _detalleOTService;
    public OrdenTrabajoViewModel(IOrdenTrabajoService ordenTrabajoService, IDialogService dialogService, IDetalleOTService detalleOTService)
        : base(ordenTrabajoService, dialogService)
    {
        _ordenTrabajoService = ordenTrabajoService;
        _detalleOTService = detalleOTService;
    }

    public override async Task Delete(long id)
    {
        OrdenTrabajo? ordenTrabajo = await _ordenTrabajoService.FindById(id);

        if (ordenTrabajo == null)
            return;

        await RunServiceAction(async () =>
        {
            bool deletedFactura = await _ordenTrabajoService.DeleteById(id);

            if (!deletedFactura)
                return false;

            return await _detalleOTService.DeleteByFolio(ordenTrabajo.Folio);
        },
        () => removeEntityById(id), $"Error al eliminar la orden de trabajo");
    }

    public async Task SincronizarDetalles(
        IList<DetalleOrdenTrabajo> originales,
        IList<DetalleOrdenTrabajo> editados,
        OrdenTrabajo ordenTrabajo)
    {
        List<long> paraEliminar = originales.Any() ? 
            [.. originales.Where(o => !editados.Any(e => e.Id == o.Id)).Select(o => o.Id)]
            : [];

        List<DetalleOrdenTrabajo> paraAgregar = [.. editados.Where(e => e.Id == 0)];

        List<DetalleOrdenTrabajo> paraActualizar = [.. editados.Where(e => e.Id != 0 && originales.Any(o => o.Id == e.Id))];

        if (paraAgregar.Count != 0 || paraActualizar.Count != 0 || paraEliminar.Count != 0)
        {
            await MakeCrud(AsignarInfo(ordenTrabajo.Folio, paraAgregar),
                AsignarInfo(ordenTrabajo.Folio, paraActualizar),
                paraEliminar);
        }
    }

    private async Task MakeCrud(List<DetalleOrdenTrabajo> paraAgregar, 
        List<DetalleOrdenTrabajo> paraActualizar, 
        List<long> paraEliminar)
    {
        if (paraEliminar.Count != 0)
        {
            await RunServiceAction(() => _detalleOTService.DeleteByIds(paraEliminar), null, $"Error al eliminar los detalles de la orden de trabajo");
        }

        if (paraAgregar.Count != 0)
        {
            await RunServiceAction(() => _detalleOTService.SaveAll(paraAgregar), null, $"Error al guardar los detalles de la orden de trabajo");
        }

        if (paraActualizar.Count != 0)
        {
            await RunServiceAction(() => _detalleOTService.UpdateAll(paraActualizar), null, $"Error al actualizar los detalles de la orden de trabajo");
        }
    }

    private static List<DetalleOrdenTrabajo> AsignarInfo(string folio, IList<DetalleOrdenTrabajo> detalles)
    {
        var lista = detalles.ToList();

        if (lista.Count == 0)
            return lista;

        foreach (var detalle in detalles)
        {
            detalle.Folio = folio;
        }

        return lista;
    }
}
