using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class OrdenTrabajoViewModel : EntidadViewModel<OrdenTrabajo>
{
    public ObservableCollection<OrdenTrabajo> OrdenTrabajo => Entidades;
    public OrdenTrabajoViewModel(IOrdenTrabajoService ordenTrabajoService, IDialogService dialogService)
        : base(ordenTrabajoService, dialogService) {}
}
