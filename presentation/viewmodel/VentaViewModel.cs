using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.views.util.buildersUi.data;

namespace Gestion.presentation.viewmodel;

public class VentaViewModel : EntidadViewModel<Venta>, INotifyPropertyChanged
{
    public VentaViewModel(
        IVentaService ventaService, 
        IDialogService dialogService,
        IFacturaService facturaService, 
        IOrdenTrabajoService ordenTrabajoService)
        : base(ventaService, dialogService)
    {
        _ = DataBootstrapper.LoadFoliosFacturaSearch(facturaService, SesionApp.IdEmpresa);
        _ = DataBootstrapper.LoadFoliosOTSearch(ordenTrabajoService, SesionApp.IdEmpresa);
    }

}
