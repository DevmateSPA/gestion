using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;
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

    protected override IEnumerable<IReglaNegocio<Maquina>> DefinirReglas(
        Maquina entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Maquina>(
                m => m.Descripcion,
                "La descripción de la máquina es obligatoria."),

            new UnicoRegla<Maquina>(
                existe: (m, id) =>
                    _maquinaRepository.ExisteCodigo(
                        m.Codigo,
                        m.Empresa,
                        id),

                valor: m => m.Codigo,

                mensaje: "El código de la máquina: {0}, ya existe para la empresa actual.")
        ];
    }
}