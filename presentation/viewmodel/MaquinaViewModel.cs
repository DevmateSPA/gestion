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

        public async Task LoadMaquinaWithPendingOrders()
        {
            await RunWithLoading(
                action: async () => await _maquinaService.FindMaquinaWithPendingOrders(SesionApp.IdEmpresa),
                errorMessage: _errorMessage,
                onEmpty: () => _dialogService.ShowMessage(_emptyMessage));
        }
    }
}
