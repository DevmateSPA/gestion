using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class OperarioViewModel : EntidadViewModel<Operario>, INotifyPropertyChanged
    {
        public OperarioViewModel(IOperarioService operarioService, IDialogService dialogService)
            : base(operarioService, dialogService) {}
    }
}
