using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class GuiaDespachoViewModel : EntidadViewModel<GuiaDespacho>, INotifyPropertyChanged
{
    public GuiaDespachoViewModel(IGuiaDespachoService guiaDespachoService, IDialogService dialogService)
        : base(guiaDespachoService, dialogService) {}
}
