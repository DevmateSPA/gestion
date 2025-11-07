using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class FacturaCompraProductoService : BaseService<FacturaCompraProducto>, IFacturaCompraProductoService
{
    public FacturaCompraProductoService(IFacturaCompraProductoRepository facturaCompraProductoRepository)
        :base(facturaCompraProductoRepository) { }
}