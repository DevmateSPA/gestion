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

    public async Task<long> ContarByMaquinaWhereEmpresaAndPendientes(long empresaId, string codigoMaquina)
    {
        return await _ordenTrabajoRepository.ContarByMaquinaWhereEmpresaAndPendientes(empresaId, codigoMaquina);
    }

    public async Task<long> ContarPendientes(long empresaId)
    {
        return await _ordenTrabajoRepository.ContarPendientes(empresaId);
    }

    public async Task<List<OrdenTrabajo>> FindAllByEmpresaAndPendiente(long empresaId)
    {
        return await _ordenTrabajoRepository.FindAllByEmpresaAndPendiente(empresaId);
    }

    public async Task<List<OrdenTrabajo>> FindAllByMaquinaWhereEmpresaAndPendiente(long empresaId, string codigoMaquina)
    {
        return await _ordenTrabajoRepository.FindAllByMaquinaWhereEmpresaAndPendiente(empresaId, codigoMaquina);
    }

    public async Task<List<OrdenTrabajo>> FindPageByEmpresaAndPendiente(long empresaId, int pageNumber, int pageSize)
    {
        return await _ordenTrabajoRepository.FindPageByEmpresaAndPendiente(empresaId, pageNumber, pageSize);
    }

    public async Task<List<OrdenTrabajo>> FindPageByMaquinaWhereEmpresaAndPendiente(long empresaId, string codigoMaquina, int pageNumber, int pageSize)
    {
        return await _ordenTrabajoRepository.FindPageByMaquinaWhereEmpresaAndPendiente(empresaId, codigoMaquina, pageNumber, pageSize);
    }
}