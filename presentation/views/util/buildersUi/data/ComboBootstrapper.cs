using Gestion.core.interfaces.service;

namespace Gestion.presentation.views.util.buildersUi.data;

public static class ComboBootstrapper
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
}