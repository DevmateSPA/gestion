using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class FacturaCompraService : BaseService<FacturaCompra>, IFacturaCompraService
{
    private readonly IFacturaCompraRepository _facturaRepository;
    public FacturaCompraService(IFacturaCompraRepository facturaCompraRepository)
        :base(facturaCompraRepository)
    {
        _facturaRepository = facturaCompraRepository;
    }

    public async Task<List<FacturaCompra>> FindAllWithDetails()
    {
        return await _facturaRepository.FindAllWithDetails();
    }
}