using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.views.util.buildersUi.data;

namespace Gestion.presentation.viewmodel;

public class NotaCreditoViewModel : EntidadViewModel<NotaCredito>, INotifyPropertyChanged
{
    public NotaCreditoViewModel(
        INotaCreditoService notaCreditoService, 
        IDialogService dialogService,
        IFacturaService facturaService)
        : base(notaCreditoService, dialogService)
    {
        _ = DataBootstrapper.LoadFoliosFacturaSearch(facturaService, SesionApp.IdEmpresa);
    }
}
