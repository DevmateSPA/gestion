using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class NotaCreditoViewModel : EntidadViewModel<NotaCredito>, INotifyPropertyChanged
{
    public NotaCreditoViewModel(INotaCreditoService notaCreditoService, IDialogService dialogService)
        : base(notaCreditoService, dialogService) {}
}
