using Gestion.core.interfaces.service;

namespace Gestion.presentation.views.util.buildersUi.data;

public static class DataBootstrapper
{
    public static async Task LoadOrdenTrabajoCombos(
        IMaquinaService maquinaService,
        IOperarioService operarioService,
        long empresaId)
    {
        var maquinas = await maquinaService
            .FindAllByEmpresa(empresaId);

        var operadores = await operarioService
            .FindAllByEmpresa(empresaId);

        ComboDataProvider.Register(
            "MAQUINAS",
            maquinas.Select(m => new { m.Codigo, m.Descripcion }));

        ComboDataProvider.Register(
            "OPERADORES",
            operadores.Select(o => new { o.Codigo, o.Nombre }));
    }

    public static async Task LoadClientesSearch(
        IClienteService clienteService,
        long empresaId)
    {
        SearchDataProvider.Register(
            "RUT_CLIENTE",
            async query =>
                await clienteService.GetRutList(query, empresaId));
    }

    public static async Task LoadFoliosFacturaSearch(
        IFacturaService facturaService,
        long empresaId)
    {
        SearchDataProvider.Register(
            "FOLIO_FACTURA",
            async query =>
                await facturaService.GetFolioList(query, empresaId));
    }

    public static async Task LoadFoliosOTSearch(
        IOrdenTrabajoService ordenTrabajoService,
        long empresaId)
    {
        SearchDataProvider.Register(
            "FOLIO_OT",
            async query =>
                await ordenTrabajoService.GetFolioList(query, empresaId));
    }

    public static async Task LoadFoliosGuiaDespachoSearch(
        IGuiaDespachoService guiaDespachoService,
        long empresaId)
    {
        SearchDataProvider.Register(
            "FOLIO_GD",
            async query =>
                await guiaDespachoService.GetFolioList(query, empresaId));
    }
}