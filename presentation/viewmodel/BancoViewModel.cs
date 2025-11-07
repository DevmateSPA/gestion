using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class BancoViewModel : EntidadViewModel<Banco>
{
    public ObservableCollection<Banco> Bancos => Entidades;
    public BancoViewModel(IBancoService bancoService, IDialogService dialogService)
        : base(bancoService, dialogService) {}
}
