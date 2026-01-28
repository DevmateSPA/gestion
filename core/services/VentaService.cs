using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class VentaService : BaseService<Venta>, IVentaService
{
    private readonly IVentaRepository _ventaRepository;
    public VentaService(IVentaRepository ventaRepository)
        :base(ventaRepository)
    {
        _ventaRepository = ventaRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        Venta entity,
        long? excludeId = null)
    {
        List<string> erroresEncontrados = [];

        if (await _ventaRepository.ExisteFolio(
                folio: entity.Folio,
                empresaId: entity.Empresa,
                excludeId: excludeId))
            erroresEncontrados.Add($"El folio de la venta: {entity.Folio}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Folio))
            erroresEncontrados.Add("El folio de la venta es obligatorio.");

        return erroresEncontrados;
    }
}