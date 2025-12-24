using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class FacturaService : BaseService<Factura>, IFacturaService
{
    private readonly IFacturaRepository _facturaRepository;
    public FacturaService(IFacturaRepository facturaRepository)
        : base(facturaRepository)
    {
        _facturaRepository = facturaRepository;
    }

    public async Task<List<Factura>> FindAllByRutBetweenFecha(long empresaId, string rutCliente, DateTime fechaDesde, DateTime fechaHasta)
    {
        return await _facturaRepository.FindAllByRutBetweenFecha(empresaId, rutCliente, fechaDesde, fechaHasta);
    }

    protected override async Task<List<string>> ValidarReglasNegocio(Factura entity)
    {
        List<string> erroresEncontrados = [];

        if (await _facturaRepository.ExisteFolio(folio: entity.Folio, empresaId: entity.Empresa))
            erroresEncontrados.Add($"El folio de la factura: {entity.Folio}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Folio))
            erroresEncontrados.Add("El folio de la factura es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.RutCliente))
            erroresEncontrados.Add("El rut del cliente de la factura es obligatorio.");

        return erroresEncontrados;
    }
}