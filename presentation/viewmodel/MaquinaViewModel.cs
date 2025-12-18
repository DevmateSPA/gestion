using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;

namespace Gestion.presentation.viewmodel
{
    public class MaquinaViewModel : EntidadViewModel<Maquina>, INotifyPropertyChanged
    {
        private readonly IMaquinaService _maquinaService;
        public MaquinaViewModel(IMaquinaService maquinaService, IDialogService dialogService)
            : base(maquinaService, dialogService)
        {
            _maquinaService = maquinaService;
        }

        public async Task LoadAllMaquinaWithPendingOrders()
        {
            await RunWithLoading(
                action: async () => await _maquinaService.FindAllMaquinaWithPendingOrders(SesionApp.IdEmpresa),
                errorMessage: _errorMessage,
                onEmpty: () => _dialogService.ShowMessage(_emptyMessage));
        }

        public async Task LoadPageMaquinaWithPendingOrders(int page)
        {
            await LoadPagedEntities(
                serviceCall: async (p) => await _maquinaService.FindPageMaquinaWithPendingOrders(SesionApp.IdEmpresa, PageNumber, PageSize),
                page: page,
                emptyMessage: _emptyMessage,
                errorMessage: _errorMessage,
                totalCountCall: async () => await _maquinaService.ContarMaquinasConPendientes(SesionApp.IdEmpresa),
                allItemsCall: async () => await _maquinaService.FindAllMaquinaWithPendingOrders(SesionApp.IdEmpresa));
        }
    }
}
