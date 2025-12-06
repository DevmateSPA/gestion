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

    public async Task<List<FacturaCompraProducto>> FindByFolio(string folio)
    {
        return await _facturaCompraProductoRepository.FindByFolio(folio);
    }
}