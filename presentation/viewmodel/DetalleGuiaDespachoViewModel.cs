using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class DetalleGuiaDespachoViewModel : EntidadViewModel<Detalle>
{
    public ObservableCollection<Detalle> Detalles => Entidades;
    public DetalleGuiaDespachoViewModel(IDetalleGuiaDespachoService detalleGuiaDespachoService, IDialogService dialogService)
        : base(detalleGuiaDespachoService, dialogService) {}
}
