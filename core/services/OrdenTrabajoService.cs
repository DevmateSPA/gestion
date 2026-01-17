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

    public async Task<List<string>> GetFolioList(string input, long empresaId)
    {
        return await _ordenTrabajoRepository.GetFolioList(
            input,
            empresaId);
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        OrdenTrabajo entity,
        long? excludeId = null)
    {
        List<string> erroresEncontrados = [];

        if (await _ordenTrabajoRepository.ExisteFolio(
                folio: entity.Folio,
                empresaId: entity.Empresa,
                excludeId: excludeId))
            erroresEncontrados.Add($"El folio de la orden de trabajo: {entity.Folio}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Folio))
            erroresEncontrados.Add("El folio de la orden de trabajo es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.RutCliente))
            erroresEncontrados.Add("El rut del cliente de la orden de trabajo es obligatorio.");

        return erroresEncontrados;
    }

    public async Task<String> GetSiguienteFolio(long empresaId)
    {
        var ultimo = await _ordenTrabajoRepository.GetSiguienteFolio(empresaId);
        if (string.IsNullOrWhiteSpace(ultimo))
            return "1";
        if (!int.TryParse(ultimo, out var numero))
            throw new InvalidOperationException($"Folio inválido: {ultimo}");
        numero++;
        var nuevoFolio = $"{numero.ToString().PadLeft(8, '0')}";
        return $"{nuevoFolio}";
    }
}