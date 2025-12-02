using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.model.detalles;

namespace Gestion.core.services;

public class DetalleOTService : BaseService<DetalleOrdenTrabajo>, IDetalleOTService
{
    private readonly IDetalleOTRepository _detalleOTRepository;
    public DetalleOTService(IDetalleOTRepository detalleOTRepository)
        :base(detalleOTRepository)
    {
        _detalleOTRepository = detalleOTRepository;
    }

    public async Task<List<DetalleOrdenTrabajo>> FindByFolio(string folio)
    {
        return await _detalleOTRepository.FindByFolio(folio);
    }
}