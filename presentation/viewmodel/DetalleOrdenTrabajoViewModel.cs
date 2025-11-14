using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class DetalleOrdenTrabajoViewModel : EntidadViewModel<Detalle>
{
    public ObservableCollection<Detalle> Detalles => Entidades;
    public DetalleOrdenTrabajoViewModel(IDetalleOrdenTrabajoService detalleOrdenTrabajoService, IDialogService dialogService)
        : base(detalleOrdenTrabajoService, dialogService) {}
}
