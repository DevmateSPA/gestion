using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class NotaCreditoViewModel : EntidadViewModel<NotaCredito>
{
    public ObservableCollection<NotaCredito> NotaCredito => Entidades;
    public NotaCreditoViewModel(INotaCreditoService notaCreditoService, IDialogService dialogService)
        : base(notaCreditoService, dialogService) {}
}
