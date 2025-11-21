using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.Infrastructure.data;

namespace Gestion.core.services;

public class FacturaService : BaseService<Factura>, IFacturaService
{
    public FacturaService(IFacturaRepository facturaRepository)
        : base(facturaRepository)
    {}
}