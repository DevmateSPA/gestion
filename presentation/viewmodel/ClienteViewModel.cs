using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class ClienteViewModel : EntidadViewModel<Cliente>, INotifyPropertyChanged
    {
        public ObservableCollection<Cliente> Clientes => Entidades;

        private ObservableCollection<Cliente> _clientesFiltrados = new();
        public ObservableCollection<Cliente> ClientesFiltrados
        {
            get => _clientesFiltrados;
            set { _clientesFiltrados = value; OnPropertyChanged(); }
        }

        public ClienteViewModel(IClienteService clienteService, IDialogService dialogService)
            : base(clienteService, dialogService)
        {}

        public override async Task LoadAll()
        {
            await base.LoadAll();
            ClientesFiltrados = new ObservableCollection<Cliente>(Clientes);
        }

        public override async Task LoadAllByEmpresa()
        {
            await base.LoadAllByEmpresa();
            ClientesFiltrados = new ObservableCollection<Cliente>(Clientes);
        }
    }
}
