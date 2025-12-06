using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel; 

public class OrdenTrabajoViewModel : EntidadViewModel<OrdenTrabajo>, INotifyPropertyChanged
{
    public OrdenTrabajoViewModel(IOrdenTrabajoService ordenTrabajoService, IDialogService dialogService)
        : base(ordenTrabajoService, dialogService) {}
}
