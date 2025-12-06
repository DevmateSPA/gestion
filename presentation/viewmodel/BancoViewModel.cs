using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;
public class BancoViewModel : EntidadViewModel<Banco>, INotifyPropertyChanged
{
    public BancoViewModel(IBancoService bancoService, IDialogService dialogService)
        : base(bancoService, dialogService) {}
}
