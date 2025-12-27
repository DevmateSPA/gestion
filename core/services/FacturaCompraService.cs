using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class FacturaCompraService : BaseService<FacturaCompra>, IFacturaCompraService
{
    private readonly IFacturaCompraRepository _facturaCompraRepository;
    public FacturaCompraService(IFacturaCompraRepository facturaCompraRepository)
        :base(facturaCompraRepository)
    {
        _facturaCompraRepository = facturaCompraRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        FacturaCompra entity,
        long? excludeId = null)
    {
        List<string> erroresEncontrados = [];

        if (await _facturaCompraRepository.ExisteFolio(
                folio: entity.Folio,
                empresaId: entity.Empresa,
                excludeId: excludeId))
            erroresEncontrados.Add($"El folio de la factura: {entity.Folio}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Folio))
            erroresEncontrados.Add("El folio de la factura es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.RutCliente))
            erroresEncontrados.Add("El rut del cliente de la factura es obligatorio.");

        return erroresEncontrados;
    }
}