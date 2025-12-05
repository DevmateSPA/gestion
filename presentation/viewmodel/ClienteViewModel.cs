using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class ClienteViewModel : EntidadViewModel<Cliente>, INotifyPropertyChanged
    {
        public ClienteViewModel(IClienteService clienteService, IDialogService dialogService)
            : base(clienteService, dialogService)
        {}
    }
}
