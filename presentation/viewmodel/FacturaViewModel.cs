using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
namespace Gestion.presentation.viewmodel;

public class FacturaViewModel : EntidadViewModel<Factura>
{
    public FacturaViewModel(IFacturaService facturaService, IDialogService dialogService)
        : base(facturaService, dialogService) {}
}
