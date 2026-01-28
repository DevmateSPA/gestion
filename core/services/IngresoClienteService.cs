using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class IngresoClienteService : BaseService<IngresoCliente>, IIngresoClienteService
{
    private readonly IIngresoClienteRepository _ingresoClienteRepository;
    public IngresoClienteService(IIngresoClienteRepository ingresoClienteRepository)
        :base(ingresoClienteRepository)
    {
        _ingresoClienteRepository = ingresoClienteRepository;
    }

    public Task<List<IngresoCliente>> LoadPageByEmpresaAndFecha(long empresaId, DateTime desde, DateTime hasta, int pageNumber, int pageSize)
    {
        return _ingresoClienteRepository.FindPageByEmpresaAndFecha(empresaId, desde, hasta, pageNumber, pageSize);
    }

    public Task<List<IngresoCliente>> LoadAllByEmpresaAndFecha(long empresaId, DateTime desde, DateTime hasta)
    {
        return _ingresoClienteRepository.FindAllByEmpresaAndFecha(empresaId,desde,hasta);
    }

    protected override IEnumerable<IReglaNegocio<IngresoCliente>> DefinirReglas(IngresoCliente entity, long? excludeId)
    {
        throw new NotImplementedException();
    }
}