using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class OrdenCompraViewModel : EntidadViewModel<OrdenCompra>
{
    public ObservableCollection<OrdenCompra> OrdenCompra => Entidades;
    public OrdenCompraViewModel(IOrdenCompraService ordenCompraService, IDialogService dialogService)
        : base(ordenCompraService, dialogService) {}
}
