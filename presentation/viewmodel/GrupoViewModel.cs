using System.ComponentModel;
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
