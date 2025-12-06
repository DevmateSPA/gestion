using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class FotomecanicaViewModel : EntidadViewModel<Fotomecanica>, INotifyPropertyChanged
    {
        public FotomecanicaViewModel(IFotomecanicaService fotomecanicaService, IDialogService dialogService)
            : base(fotomecanicaService, dialogService) {}
    }
}
