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

    protected override async Task<List<string>> ValidarReglasNegocio(Cliente entity)
    {
        List<string> erroresEncontrados = [];

        if (await _clienteRepository.ExisteRut(rut: entity.Rut, empresaId: entity.Empresa))
            erroresEncontrados.Add($"El rut del cliente: {entity.Rut}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Rut))
            erroresEncontrados.Add("El rut del cliente es obligatorio.");

        return erroresEncontrados;
    }
}