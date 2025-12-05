using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class MaquinaViewModel : EntidadViewModel<Maquina>, INotifyPropertyChanged
    {
        public MaquinaViewModel(IMaquinaService maquinaService, IDialogService dialogService)
            : base(maquinaService, dialogService) {}
    }
}
