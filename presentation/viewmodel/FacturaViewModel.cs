using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;
using Gestion.presentation.utils;

namespace Gestion.presentation.viewmodel;

public class FacturaViewModel : EntidadViewModel<Factura>
{
    public ObservableCollection<Factura> Facturas => Entidades;
    public FacturaViewModel(IFacturaService facturaService, IDialogService dialogService)
        : base(facturaService, dialogService)
    {}  
}
