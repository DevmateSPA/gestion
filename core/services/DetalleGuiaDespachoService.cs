using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class DetalleGuiaDespachoService : BaseService<Detalle>, IDetalleGuiaDespachoService
{
    private readonly IDetalleGuiaDespachoRepository _detalleGuiaDespachoRepository;
    public DetalleGuiaDespachoService(IDetalleGuiaDespachoRepository detalleGuiaDespachoRepository)
        :base(detalleGuiaDespachoRepository)
    {
        _detalleGuiaDespachoRepository = detalleGuiaDespachoRepository;
    }

    public async Task<List<Detalle>> FindByFolio(string folio)
    {
        return await _detalleGuiaDespachoRepository.FindByFolio(folio);
    }
}