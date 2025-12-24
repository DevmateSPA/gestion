using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class FacturaCompraProductoService : BaseService<FacturaCompraProducto>, IFacturaCompraProductoService
{
    private readonly IFacturaCompraProductoRepository _facturaCompraProductoRepository;
    public FacturaCompraProductoService(IFacturaCompraProductoRepository facturaCompraProductoRepository)
        :base(facturaCompraProductoRepository)
    {
        _facturaCompraProductoRepository = facturaCompraProductoRepository;
    }

    public Task<bool> DeleteByFolio(string folio)
    {
        return _facturaCompraProductoRepository.DeleteByFolio(folio);
    }

    public async Task<bool> DeleteByIds(IList<long> ids)
    {
        return await _facturaCompraProductoRepository.DeleteByIds(ids);
    }

    public async Task<List<FacturaCompraProducto>> FindByFolio(string folio)
    {
        return await _facturaCompraProductoRepository.FindByFolio(folio);
    }

    public async Task<bool> SaveAll(List<FacturaCompraProducto> detalles)
    {
        return await _facturaCompraProductoRepository.SaveAll(detalles);
    }

    public async Task<bool> UpdateAll(IList<FacturaCompraProducto> detalles)
    {
        return await _facturaCompraProductoRepository.UpdateAll(detalles);
    }

    protected override async Task<List<string>> ValidarReglasNegocio(FacturaCompraProducto entity)
    {
        List<string> erroresEncontrados = [];
        return erroresEncontrados;
    }
}