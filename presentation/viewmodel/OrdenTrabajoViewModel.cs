using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.model.detalles;
using Gestion.core.session;
using Gestion.helpers;
using Gestion.presentation.views.util.buildersUi;
using Gestion.presentation.views.util.buildersUi.data;

namespace Gestion.presentation.viewmodel; 

public class OrdenTrabajoViewModel : EntidadViewModel<OrdenTrabajo>, INotifyPropertyChanged
{
    private readonly IOrdenTrabajoService _ordenTrabajoService;
    private readonly IDetalleOTService _detalleOTService;
    private readonly IMaquinaService _maquinaService;
    private readonly IOperarioService _operarioService;
    public OrdenTrabajoViewModel(
        IOrdenTrabajoService ordenTrabajoService, 
        IDialogService dialogService, 
        IDetalleOTService detalleOTService,
        IMaquinaService maquinaService,
        IOperarioService operarioService)
        : base(ordenTrabajoService, dialogService)
    {
        _ordenTrabajoService = ordenTrabajoService;
        _detalleOTService = detalleOTService;
        _maquinaService = maquinaService;
        _operarioService = operarioService;

        _ = DataBootstrapper.LoadOrdenTrabajoCombos(_maquinaService, _operarioService, SesionApp.IdEmpresa);
    }

    public virtual async Task<List<DetalleOrdenTrabajo>> LoadDetailsByFolio(string folio)
    {
        List<DetalleOrdenTrabajo>? detalles = null;

        await SafeExecutor.RunAsync(
            async () => detalles = await _detalleOTService.FindByFolio(folio),
            _dialogService,
            $"Error al cargar los detalles de la orden de trabajo con folio: {folio}");
            
        return detalles ?? [];
    }

    public virtual async Task LoadAllByMaquinaWhereEmpresaAndPendiente(string codigoMaquina)
    {
        await RunWithLoading(
            action: async () => await _ordenTrabajoService.FindAllByMaquinaWhereEmpresaAndPendiente(SesionApp.IdEmpresa, codigoMaquina),
            errorMessage: _errorMessage,
            onEmpty: () => _dialogService.ShowMessage(_emptyMessage));
    }

    public virtual async Task LoadPageByMaquinaWhereEmpresaAndPendiente(int page, string codigoMaquina)
    {
        await LoadPagedEntities(
            serviceCall: async (p) => await _ordenTrabajoService.FindPageByMaquinaWhereEmpresaAndPendiente(SesionApp.IdEmpresa, codigoMaquina, p, PageSize),
            page: page,
            emptyMessage: _emptyMessage,
            errorMessage: _errorMessage);

        await LoadPagedEntities(
            serviceCall: async (p) => await _ordenTrabajoService.FindPageByMaquinaWhereEmpresaAndPendiente(SesionApp.IdEmpresa, codigoMaquina, p, PageSize),
            page: page,
            emptyMessage: _emptyMessage,
            errorMessage: _errorMessage,
            totalCountCall: async () => await _ordenTrabajoService.ContarByMaquinaWhereEmpresaAndPendientes(SesionApp.IdEmpresa, codigoMaquina),
            allItemsCall: async () => await _ordenTrabajoService.FindAllByMaquinaWhereEmpresaAndPendiente(SesionApp.IdEmpresa, codigoMaquina));
    }

    public override async Task Delete(long id)
    {
        OrdenTrabajo? ordenTrabajo = await _ordenTrabajoService.FindById(id);

        if (ordenTrabajo == null)
            return;

        await RunServiceAction(async () =>
        {
            bool ot = await _ordenTrabajoService.DeleteById(id);
            bool detalles = false;

            if (ot)
                detalles = await _detalleOTService.DeleteByFolio(ordenTrabajo.Folio);

            return ot; 
        },
        () => RemoveEntityById(id),
        "Error al eliminar la orden de trabajo");
    }

    public async Task SincronizarDetalles(
        IEnumerable<DetalleOrdenTrabajo> originales,
        IEnumerable<DetalleOrdenTrabajo> editados,
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

    // Pendientes Entrega

    public async Task LoadAllByEmpresaAndPendiente()
    {
        await RunWithLoading(
            action: async () => await _ordenTrabajoService.FindAllByEmpresaAndPendiente(SesionApp.IdEmpresa),
            errorMessage: _errorMessage,
            onEmpty: () => _dialogService.ShowMessage(_emptyMessage));
    }

    public async Task LoadPageByEmpresaAndPendiente(int page)
    {
        await LoadPagedEntities(
            serviceCall: async (p) => await _ordenTrabajoService.FindPageByEmpresaAndPendiente(SesionApp.IdEmpresa, PageNumber, PageSize),
            page: page,
            emptyMessage: _emptyMessage,
            errorMessage: _errorMessage,
            totalCountCall: async () => await _ordenTrabajoService.ContarPendientes(SesionApp.IdEmpresa),
            allItemsCall: async () => await _ordenTrabajoService.FindAllByEmpresaAndPendiente(SesionApp.IdEmpresa));
    }
}