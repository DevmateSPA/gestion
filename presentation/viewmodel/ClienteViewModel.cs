using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class ClienteViewModel : Window
{
    private readonly IClienteService _clienteService;
    public ObservableCollection<Cliente> Clientes { get; set; } = new ObservableCollection<Cliente>();
    public ClienteViewModel(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }
    
    public async Task LoadClientes()
    {
        var lista = await _clienteService.FindAll();
        Clientes.Clear();
        foreach (var cliente in lista)
            Clientes.Add(cliente);
    }
}
