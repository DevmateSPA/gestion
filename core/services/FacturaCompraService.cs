using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class FacturaCompraService : BaseService<FacturaCompra>, IFacturaCompraService
{
    public FacturaCompraService(IFacturaCompraRepository facturaCompraRepository)
        :base(facturaCompraRepository)
    {}
}