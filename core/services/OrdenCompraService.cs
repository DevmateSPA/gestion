using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class OrdenCompraService : BaseService<OrdenCompra>, IOrdenCompraService
{
    public OrdenCompraService(IOrdenCompraRepository ordenCompraRepository)
        :base(ordenCompraRepository) { }
}