using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class DetalleOrdenTrabajoService : BaseService<Detalle>, IDetalleOrdenTrabajoService
{
    private readonly IDetalleOrdenTrabajoRepository _detalleOrdenTrabajoSRepository;
    public DetalleOrdenTrabajoService(IDetalleOrdenTrabajoRepository detalleOrdenTrabajoSRepository)
        :base(detalleOrdenTrabajoSRepository)
    {
        _detalleOrdenTrabajoSRepository = detalleOrdenTrabajoSRepository;
    }

    public async Task<List<Detalle>> FindByFolio(string folio)
    {
        return await _detalleOrdenTrabajoSRepository.FindByFolio(folio);
    }
}