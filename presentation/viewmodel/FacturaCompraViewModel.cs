using System.Collections.ObjectModel;
using System.Reflection;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;
using Gestion.presentation.utils;

namespace Gestion.presentation.viewmodel;

public class FacturaCompraViewModel : EntidadViewModel<FacturaCompra>
{
    public ObservableCollection<FacturaCompra> FacturasCompra => Entidades;
    public FacturaCompraViewModel(IFacturaCompraService facturaCompraService, IDialogService dialogService)
        : base(facturaCompraService, dialogService)
    {}
}
