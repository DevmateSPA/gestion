using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class EmpresaService : BaseService<Empresa>, IEmpresaService
{
    public EmpresaService(IEmpresaRepository empresaRepository)
        :base(empresaRepository) { }
}