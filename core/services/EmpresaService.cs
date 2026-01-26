using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class EmpresaService : BaseService<Empresa>, IEmpresaService
{
    public EmpresaService(IEmpresaRepository empresaRepository)
        :base(empresaRepository) { }

    protected override IEnumerable<IReglaNegocio<Empresa>> DefinirReglas(
        Empresa entity,
        long? excludeId = null)
    {
        return [];
    }
}