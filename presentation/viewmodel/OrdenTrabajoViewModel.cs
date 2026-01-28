using System.ComponentModel;
using System.Diagnostics;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.model.detalles;
using Gestion.core.session;
using Gestion.helpers;
using Gestion.presentation.views.util.buildersUi.data;

namespace Gestion.presentation.viewmodel; 

public class OrdenTrabajoViewModel : EntidadViewModel<OrdenTrabajo>, INotifyPropertyChanged
{
    private readonly IOrdenTrabajoService _ordenTrabajoService;
    private readonly IDetalleOTService _detalleOTService;
    public OrdenTrabajoViewModel(
        IOrdenTrabajoService ordenTrabajoService, 
        IDialogService dialogService, 
        IDetalleOTService detalleOTService,
        IMaquinaService maquinaService,
        IOperarioService operarioService,
        IClienteService clienteService)
        : base(ordenTrabajoService, dialogService)
    {
        _ordenTrabajoService = ordenTrabajoService;
        _detalleOTService = detalleOTService;

        _ = DataBootstrapper.LoadOrdenTrabajoCombos(maquinaService, operarioService, SesionApp.IdEmpresa);

        _ = DataBootstrapper.LoadClientesSearch(clienteService, SesionApp.IdEmpresa);
    }

    public virtual async Task<List<DetalleOrdenTrabajo>> LoadDetailsByFolio(string folio, long empresaId)
    {
        List<DetalleOrdenTrabajo>? detalles = null;

        await SafeExecutor.RunAsync(
            async () => detalles = await _detalleOTService.FindByFolio(folio, empresaId),
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
                detalles = await _detalleOTService.DeleteByFolio(ordenTrabajo.Folio, ordenTrabajo.Empresa);

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
        Debug.WriteLine("======================================");
        Debug.WriteLine("[SincronizarDetalles] INICIO");
        Debug.WriteLine($"OT: {ordenTrabajo.Folio} | Empresa: {ordenTrabajo.Empresa}");

        Debug.WriteLine("Originales:");
        foreach (var o in originales)
            Debug.WriteLine($"  O -> Id={o.Id}");

        Debug.WriteLine("Editados:");
        foreach (var e in editados)
            Debug.WriteLine($"  E -> Id={e.Id}");

        List<long> paraEliminar = originales.Any()
            ? [.. originales
                .Where(o => !editados.Any(e => e.Id == o.Id))
                .Select(o => o.Id)]
            : [];

        List<DetalleOrdenTrabajo> paraAgregar =
            [.. editados.Where(e => e.Id == 0)];

        List<DetalleOrdenTrabajo> paraActualizar =
        [
            .. editados.Where(e =>
            {
                var original = originales.FirstOrDefault(o => o.Id == e.Id);
                return original != null && !original.Equals(e);
            })
        ];

        Debug.WriteLine("Resultado sincronización:");
        Debug.WriteLine($" - Para eliminar: {(paraEliminar.Count != 0 ? string.Join(", ", paraEliminar) : "(ninguno)")}");
        Debug.WriteLine($" - Para agregar: {(paraAgregar.Count != 0 ? string.Join(", ", paraAgregar.Select(a => a.Id)) : "(ninguno)")}");
        Debug.WriteLine($" - Para actualizar: {(paraActualizar.Count != 0 ? string.Join(", ", paraActualizar.Select(a => a.Id)) : "(ninguno)")}");

        if (paraEliminar.Count == 0 && paraAgregar.Count == 0 && paraActualizar.Count == 0)
        {
            Debug.WriteLine(
                $"[SincronizarDetalles] No hay cambios: {originales.Count()} detalles comparados, 0 diferencias detectadas"
            );
            Debug.WriteLine("[SincronizarDetalles] FIN");
            Debug.WriteLine("======================================");
            return;
        }

        Debug.WriteLine("[SincronizarDetalles] Ejecutando CRUD...");

        await MakeCrud(
            AsignarInfo(ordenTrabajo.Folio, ordenTrabajo.Empresa, paraAgregar),
            AsignarInfo(ordenTrabajo.Folio, ordenTrabajo.Empresa, paraActualizar),
            paraEliminar,
            ordenTrabajo.Empresa
        );

        Debug.WriteLine("[SincronizarDetalles] FIN");
        Debug.WriteLine("======================================");
    }

    private async Task MakeCrud(List<DetalleOrdenTrabajo> paraAgregar, 
        List<DetalleOrdenTrabajo> paraActualizar, 
        List<long> paraEliminar,
        long empresaId)
    {
        Debug.WriteLine("[MakeCrud] Iniciando CRUD...");

        if (paraEliminar.Count != 0)
        {
            await RunServiceAction(() => _detalleOTService.DeleteByIds(paraEliminar, empresaId), null, $"Error al eliminar los detalles de la orden de trabajo");
        }

        if (paraAgregar.Count != 0)
        {
            Debug.WriteLine($" - Agregando IDs: {string.Join(", ", paraAgregar.Select(a => a.Id))}");
            await RunServiceAction(() => _detalleOTService.SaveAll(paraAgregar), null, "Error al guardar los detalles de la orden de trabajo");
        }

        if (paraActualizar.Count != 0)
        {
            await RunServiceAction(() => _detalleOTService.UpdateAll(paraActualizar, empresaId), null, $"Error al actualizar los detalles de la orden de trabajo");
        }

        Debug.WriteLine("[MakeCrud] Operación finalizada.");
    }

    private static List<DetalleOrdenTrabajo> AsignarInfo(string folio, long empresaId, IList<DetalleOrdenTrabajo> detalles)
    {
        var lista = detalles.ToList();

        if (lista.Count == 0)
            return lista;

        foreach (DetalleOrdenTrabajo detalle in detalles)
        {
            detalle.Empresa = empresaId;
            detalle.Folio = folio;
            detalle.Empresa = empresaId;
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

    public Task<String> GetSiguienteFolio()
    {
        return _ordenTrabajoService.GetSiguienteFolio(SesionApp.IdEmpresa);
    }

}
