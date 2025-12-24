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

    public Task<bool> DeleteByFolio(string folio)
    {
        return _detalleOTRepository.DeleteByFolio(folio);
    }

    public async Task<bool> DeleteByIds(IList<long> ids)
    {
        return await _detalleOTRepository.DeleteByIds(ids);
    }

    public async Task<bool> SaveAll(List<DetalleOrdenTrabajo> detalles)
    {
        return await _detalleOTRepository.SaveAll(detalles);
    }

    public async Task<bool> UpdateAll(IList<DetalleOrdenTrabajo> detalles)
    {
        return await _detalleOTRepository.UpdateAll(detalles);
    }

    protected override async Task<List<string>> ValidarReglasNegocio(DetalleOrdenTrabajo entity)
    {
        List<string> erroresEncontrados = [];
        return erroresEncontrados;
    }
}