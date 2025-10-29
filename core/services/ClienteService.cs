using Gestion.core.interfaces;
using Gestion.core.model;

namespace Gestion.core.services;

public class ClienteService : IClienteService
{
    private readonly IClienteRepository _clientRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clientRepository = clienteRepository;
    }

    public async Task<List<Cliente>> GetClientes()
    {
        return await _clientRepository.GetClientes();
    }
}