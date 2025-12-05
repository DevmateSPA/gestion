using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel
{
    public class FotomecanicaViewModel : EntidadViewModel<Fotomecanica>, INotifyPropertyChanged
    {
        public FotomecanicaViewModel(IFotomecanicaService fotomecanicaService, IDialogService dialogService)
            : base(fotomecanicaService, dialogService) {}
    }
}
