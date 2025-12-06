using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class EncuadernacionViewModel : EntidadViewModel<Encuadernacion>, INotifyPropertyChanged
    {
        public EncuadernacionViewModel(IEncuadernacionService encuadernacionService, IDialogService dialogService)
            : base(encuadernacionService, dialogService) {}
    }
}
