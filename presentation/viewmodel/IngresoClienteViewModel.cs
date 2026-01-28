using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;

namespace Gestion.presentation.viewmodel
{
    public class IngresoClienteViewModel : EntidadViewModel<IngresoCliente>, INotifyPropertyChanged
    {
        private readonly IIngresoClienteService _ingresoClienteService;
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public IngresoClienteViewModel(
            IIngresoClienteService ingresoClienteService,
            IDialogService dialogService)
            : base(ingresoClienteService, dialogService)
        {
            _ingresoClienteService = ingresoClienteService;
        }

        public async Task LoadAllByEmpresaAndFecha()
        {
            await RunWithLoading(
                action: async () => await _ingresoClienteService.LoadAllByEmpresaAndFecha(SesionApp.IdEmpresa,FechaDesde, FechaHasta),
                errorMessage: _errorMessage,
                onEmpty: () => _dialogService.ShowMessage(_emptyMessage));
        }

        public async Task LoadPageByEmpresaAndFecha(int page)
        {
            await LoadPagedEntities(
                serviceCall: async (p) => await _ingresoClienteService.LoadPageByEmpresaAndFecha(SesionApp.IdEmpresa,FechaDesde, FechaHasta, PageNumber, PageSize),
                page: page,
                emptyMessage: _emptyMessage,
                errorMessage: _errorMessage);
        }

    }
}
