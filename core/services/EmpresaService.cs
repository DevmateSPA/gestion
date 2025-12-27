using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class EmpresaService : BaseService<Empresa>, IEmpresaService
{
    public EmpresaService(IEmpresaRepository empresaRepository)
        :base(empresaRepository) { }

    protected override async Task<List<string>> ValidarReglasNegocio(
        Empresa entity,
        long? excludeId = null)
    {
        throw new NotImplementedException();
    }
}