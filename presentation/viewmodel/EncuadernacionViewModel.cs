using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel
{
    public class EncuadernacionViewModel : EntidadViewModel<Encuadernacion>, INotifyPropertyChanged
    {
        public EncuadernacionViewModel(IEncuadernacionService encuadernacionService, IDialogService dialogService)
            : base(encuadernacionService, dialogService) {}
    }
}
