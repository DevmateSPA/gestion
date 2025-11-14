using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class DetalleViewModel : EntidadViewModel<Detalle>
{
    public ObservableCollection<Detalle> Detalles => Entidades;
    public DetalleViewModel(IDetalleService detalleService, IDialogService dialogService)
        : base(detalleService, dialogService) {}
}
