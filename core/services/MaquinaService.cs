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

    public async Task<long> ContarMaquinasConPendientes(long empresaId)
    {
        return await _maquinaRepository.ContarMaquinasConPendientes(empresaId);
    }

    public async Task<List<Maquina>> FindAllMaquinaWithPendingOrders(long empresaId)
    {
        return await _maquinaRepository.FindAllMaquinaWithPendingOrders(empresaId);
    }

    public async Task<List<Maquina>> FindPageMaquinaWithPendingOrders(long empresaId, int pageNumber, int pageSize)
    {
        return await _maquinaRepository.FindPageMaquinaWithPendingOrders(empresaId, pageNumber, pageSize);
    }

    protected override async Task<List<string>> ValidarReglasNegocio(Maquina entity)
    {
        List<string> erroresEncontrados = [];

        if (await _maquinaRepository.ExisteCodigo(codigo: entity.Codigo, empresaId: entity.Empresa))
            erroresEncontrados.Add($"El código de la máquina: {entity.Codigo}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Descripcion))
            erroresEncontrados.Add("La descripción de la máquina es obligatoria.");

        return erroresEncontrados;
    }
}