using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.views.util.buildersUi.data;

namespace Gestion.presentation.viewmodel;

public class GuiaDespachoViewModel : EntidadViewModel<GuiaDespacho>, INotifyPropertyChanged
{
    public GuiaDespachoViewModel(
        IGuiaDespachoService guiaDespachoService, 
        IDialogService dialogService,
        IClienteService clienteService,
        IFacturaService facturaService)
        : base(guiaDespachoService, dialogService)
    {
        
        _ = DataBootstrapper.LoadClientesSearch(clienteService, SesionApp.IdEmpresa);
        _ = DataBootstrapper.LoadFoliosSearch(facturaService, SesionApp.IdEmpresa);
    }
}
