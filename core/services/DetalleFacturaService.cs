using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class DetalleFacturaService : BaseService<Detalle>, IDetalleFacturaService
{
    private readonly IDetalleFacturaRepository _detalleFacturaRepository;
    public DetalleFacturaService(IDetalleFacturaRepository detalleFacturaRepository)
        :base(detalleFacturaRepository)
    {
        _detalleFacturaRepository = detalleFacturaRepository;
    }

    public async Task<List<Detalle>> FindByFolio(string folio)
    {
        return await _detalleFacturaRepository.FindByFolio(folio);
    }
}