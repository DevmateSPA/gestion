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

    public async Task<List<DetalleOrdenTrabajo>> FindByFolio(
        string folio, 
        long empresaId)
    {
        return await _detalleOTRepository.FindByFolio(folio, empresaId);
    }

    public Task<bool> DeleteByFolio(
        string folio, 
        long empresaId)
    {
        return _detalleOTRepository.DeleteByFolio(folio, empresaId);
    }

    public async Task<bool> DeleteByIds(
        IList<long> ids, 
        long empresaId)
    {
        return await _detalleOTRepository.DeleteByIds(ids, empresaId);
    }

    public async Task<bool> SaveAll(List<DetalleOrdenTrabajo> detalles)
    {
        return await _detalleOTRepository.SaveAll(detalles);
    }

    public async Task<bool> UpdateAll(
        IList<DetalleOrdenTrabajo> detalles, 
        long empresaId)
    {
        return await _detalleOTRepository.UpdateAll(detalles, empresaId);
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        DetalleOrdenTrabajo entity,
        long? excludeId = null)
    {
        List<string> erroresEncontrados = [];
        return erroresEncontrados;
    }
}