using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.views.util.buildersUi.data;

namespace Gestion.presentation.viewmodel;

public class NotaCreditoViewModel : EntidadViewModel<NotaCredito>, INotifyPropertyChanged
{
    private readonly INotaCreditoService _notaCreditoService;
    public NotaCreditoViewModel(
        INotaCreditoService notaCreditoService, 
        IDialogService dialogService,
        IFacturaService facturaService)
        : base(notaCreditoService, dialogService)
    {
        _notaCreditoService = notaCreditoService;
        
        _ = DataBootstrapper.LoadFoliosFacturaSearch(facturaService, SesionApp.IdEmpresa);
    }

    public Task<string> GetSiguienteFolio()
    {
        return _notaCreditoService.GetSiguienteFolio(SesionApp.IdEmpresa);
    }
}
