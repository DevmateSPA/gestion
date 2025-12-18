using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;
namespace Gestion.presentation.viewmodel;

public class FacturaViewModel : EntidadViewModel<Factura>
{
    private readonly IFacturaService _facturaService;
    public FacturaViewModel(IFacturaService facturaService, IDialogService dialogService)
        : base(facturaService, dialogService)
    {
        _facturaService = facturaService;
    }

    public async Task LoadAllByRutClienteBetweenFecha(string rutCliente, DateTime fechaDesde, DateTime fechaHasta)
    {
        await RunWithLoading(
            action: async () => await _facturaService.FindAllByRutBetweenFecha(SesionApp.IdEmpresa, rutCliente, fechaDesde, fechaHasta),
            errorMessage: _errorMessage,
            onEmpty: () => _dialogService.ShowMessage(_emptyMessage));
    }
}
