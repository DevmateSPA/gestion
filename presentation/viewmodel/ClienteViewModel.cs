using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class ClienteViewModel : EntidadViewModel<Cliente>
{
    public ObservableCollection<Cliente> Clientes => Entidades;
    public ClienteViewModel(IClienteService clienteService, IDialogService dialogService)
        : base(clienteService, dialogService) {}
}