using System.Data.Common;
using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;
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

    public async Task<List<string>> GetFolioList(string input, long empresaId)
    {
        return await _ordenTrabajoRepository.GetFolioList(
            input,
            empresaId);
    }

    protected override IEnumerable<IReglaNegocio<OrdenTrabajo>> DefinirReglas(
        OrdenTrabajo entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<OrdenTrabajo>(
                c => c.Folio,
                "El folio de la orden de trabajo es obligatorio."),

            new RequeridoRegla<OrdenTrabajo>(
                c => c.RutCliente,
                "El rut del cliente de la orden de trabajo es obligatorio."),

            new RequeridoRegla<OrdenTrabajo>(
                o => o.Maquina1,
                "Debe seleccionar una Máquina principal."),

            new RequeridoRegla<OrdenTrabajo>(
                o => o.Operador1,
                "Debe seleccionar un Operador principal."),

            new UnicoRegla<OrdenTrabajo>(
                existe: (ot, id) =>
                    _ordenTrabajoRepository.ExistsByColumns(
                        [
                            ("folio", ot.Folio),
                            ("empresa", ot.Empresa)
                        ],
                        id),

                valor: ot => ot.Folio,

                mensaje: "El folio de la orden de trabajo: {0}, ya existe para la empresa actual.")
        ];
    }

    public async Task<string> GetSiguienteFolio(long empresaId)
    {
        return await _ordenTrabajoRepository.GetSiguienteFolio(empresaId);
    }
}