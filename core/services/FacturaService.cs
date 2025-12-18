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
}