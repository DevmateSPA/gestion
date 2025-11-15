using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.Infrastructure.data;

namespace Gestion.core.services;

public class FacturaService : BaseService<Factura>, IFacturaService
{
    private readonly IFacturaRepository _facturaRepository;

    public FacturaService(IFacturaRepository facturaRepository)
        : base(facturaRepository)
    {
        _facturaRepository = facturaRepository;
    }

    public async Task<List<Factura>> FindAllWithDetails()
    {
        return await _facturaRepository.FindAllWithDetails();
    }
}