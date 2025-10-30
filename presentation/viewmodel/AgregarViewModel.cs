using Gestion.core.interfaces;
using Gestion.core.model;
using Gestion.core.services;

namespace Gestion.presentation.viewmodel;

public class AgregarViewModel
{
    private readonly IClienteService _clienteService;

    public AgregarViewModel(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    public async Task<List<Cliente>> GetClientes()
    {
        return await _clienteService.GetClientes();
    }
}