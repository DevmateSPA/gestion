using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
