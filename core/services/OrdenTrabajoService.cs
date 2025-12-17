using System.Data.Common;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.core.services;

public class OrdenTrabajoService : BaseService<OrdenTrabajo>, IOrdenTrabajoService
{
    private readonly IOrdenTrabajoRepository _ordenTrabajoRepository;
    public OrdenTrabajoService(IOrdenTrabajoRepository ordenTrabajoRepository)
        :base(ordenTrabajoRepository)
    {
        _ordenTrabajoRepository = ordenTrabajoRepository;
    }

    public async Task<List<OrdenTrabajo>> FindPageByEmpresaAndPendiente(long empresaId, int pageNumber, int pageSize)
    {
        return await _ordenTrabajoRepository.FindPageByEmpresaAndPendiente(empresaId, pageNumber, pageSize);
    }
}