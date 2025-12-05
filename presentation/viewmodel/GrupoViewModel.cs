using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class GrupoViewModel : EntidadViewModel<Grupo>, INotifyPropertyChanged
    {
        public GrupoViewModel(IGrupoService grupoService, IDialogService dialogService)
            : base(grupoService, dialogService) {}
    }
}
