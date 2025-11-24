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

    public Task<List<Factura>> FindAllByEmpresa(long empresaId)
    {
        return _facturaRepository.FindAllByEmpresa(empresaId);
    }
}