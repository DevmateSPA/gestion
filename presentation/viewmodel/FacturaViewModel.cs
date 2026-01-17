using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.presentation.views.util.buildersUi.data;
namespace Gestion.presentation.viewmodel;

public class FacturaViewModel : EntidadViewModel<Factura>
{
    private readonly IFacturaService _facturaService;
    public FacturaViewModel(
        IFacturaService facturaService, 
        IDialogService dialogService,
        IOrdenTrabajoService ordenTrabajoService,
        IGuiaDespachoService guiaDespachoService)
        : base(facturaService, dialogService)
    {
        _facturaService = facturaService;

        _ = DataBootstrapper.LoadFoliosOTSearch(ordenTrabajoService, SesionApp.IdEmpresa);
        _ = DataBootstrapper.LoadFoliosGuiaDespachoSearch(guiaDespachoService, SesionApp.IdEmpresa);
    }

    public async Task LoadAllByRutClienteBetweenFecha(string rutCliente, DateTime fechaDesde, DateTime fechaHasta)
    {
        await RunWithLoading(
            action: async () => await _facturaService.FindAllByRutBetweenFecha(SesionApp.IdEmpresa, rutCliente, fechaDesde, fechaHasta),
            errorMessage: _errorMessage,
            onEmpty: () => _dialogService.ShowMessage(_emptyMessage));
    }

    public Task<String> GetSiguienteFolio()
    {
        return _facturaService.GetSiguienteFolio(SesionApp.IdEmpresa);
    }
}
