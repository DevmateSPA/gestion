using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class ImpresionViewModel : EntidadViewModel<Impresion>, INotifyPropertyChanged
    {
        public ImpresionViewModel(IImpresionService impresionService, IDialogService dialogService)
            : base(impresionService, dialogService) {}
    }
}
