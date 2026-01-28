using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class VentaService : BaseService<Venta>, IVentaService
{
    private readonly IVentaRepository _ventaRepository;
    public VentaService(IVentaRepository ventaRepository)
        :base(ventaRepository)
    {
        _ventaRepository = ventaRepository;
    }

    protected override IEnumerable<IReglaNegocio<Venta>> DefinirReglas(Venta entity, long? excludeId)
    {
        return [];
    }

}