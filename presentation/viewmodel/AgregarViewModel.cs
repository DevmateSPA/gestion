using Gestion.core.interfaces.service;
using Gestion.core.model;

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
        return await _clienteService.FindAll();
    }
}