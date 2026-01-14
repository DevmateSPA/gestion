using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class ClienteService : BaseService<Cliente>, IClienteService
{
    private readonly IClienteRepository _clienteRepository;
    public ClienteService(IClienteRepository clienteRepository)
        :base(clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<List<string>> GetRutList(string busquedaRut, long empresaId)
    {
        return await _clienteRepository.GetRutList(busquedaRut, empresaId);
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        Cliente entity,
        long? excludeId = null)
    {
        List<string> erroresEncontrados = [];

        if (await _clienteRepository.ExisteRut(
                rut: entity.Rut,
                empresaId: entity.Empresa,
                excludeId: excludeId))
            erroresEncontrados.Add($"El rut del cliente: {entity.Rut}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Rut))
            erroresEncontrados.Add("El rut del cliente es obligatorio.");

        return erroresEncontrados;
    }
}