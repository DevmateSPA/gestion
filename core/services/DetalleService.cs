using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class DetalleService : BaseService<Detalle>, IDetalleService
{
    private readonly IDetalleRepository _detalleRepository;
    public DetalleService(IDetalleRepository detalleRepository)
        :base(detalleRepository)
    {
        _detalleRepository = detalleRepository;
    }

    public async Task<List<Detalle>> FindByFolio(string folio)
    {
        return await _detalleRepository.FindByFolio(folio);
    }
}