using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class ClienteViewModel
{
    private readonly IClienteService _clienteService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Cliente> Clientes { get; set; } = new ObservableCollection<Cliente>();
    public ClienteViewModel(IClienteService clienteService, IDialogService dialogService)
    {
        _clienteService = clienteService;
        _dialogService = dialogService;

    }

    public async Task updateCliente(Cliente cliente)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            await _clienteService.Update(cliente);

            var index = Clientes.IndexOf(Clientes.FirstOrDefault(b => b.Id == cliente.Id));
            if (index >= 0)
                Clientes[index] = cliente;


        }, _dialogService, "Error al actualizar el cliente");
    }
    
    public async Task LoadClientes()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _clienteService.FindAll();
            Clientes.Clear();
            foreach (var cliente in lista)
                Clientes.Add(cliente);
        }, _dialogService, "Error al cargar los clientes");
    }
}
