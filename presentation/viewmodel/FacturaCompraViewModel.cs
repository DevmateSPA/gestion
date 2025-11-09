using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class FacturaCompraViewModel : EntidadViewModel<FacturaCompra>
{
    public ObservableCollection<FacturaCompra> FacturasCompra => Entidades;
    public FacturaCompraViewModel(IFacturaCompraService facturaCompraService, IDialogService dialogService)
        : base(facturaCompraService, dialogService) {}
}
