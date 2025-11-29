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

        private string _filtro = "";
        public string Filtro
        {
            get => _filtro;
            set { _filtro = value; OnPropertyChanged(); }
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

        public void Buscar()
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                ClientesFiltrados = new ObservableCollection<Cliente>(Clientes);
                return;
            }

            var lower = Filtro.ToLower();

            ClientesFiltrados = new ObservableCollection<Cliente>(
                Clientes.Where(c =>
                       (c.Razon_Social?.ToLower().Contains(lower) ?? false)
                    || (c.Rut?.ToLower().Contains(lower) ?? false)
                    || (c.Giro?.ToLower().Contains(lower) ?? false)
                )
            );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
