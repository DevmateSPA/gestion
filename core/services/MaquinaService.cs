using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.Infrastructure.data;

namespace Gestion.core.services;

public class MaquinaService : BaseService<Maquina>, IMaquinaService
{
    private readonly IMaquinaRepository _maquinaRepository;
    public MaquinaService(IMaquinaRepository maquinaRepository)
        :base(maquinaRepository)
    {
        _maquinaRepository = maquinaRepository;
    }

    public async Task<List<Maquina>> FindMaquinaWithPendingOrders(long empresaId)
    {
        return await _maquinaRepository.FindMaquinaWithPendingOrders(empresaId);
    }
}