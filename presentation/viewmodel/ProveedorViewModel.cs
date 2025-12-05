using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class ProveedorViewModel : EntidadViewModel<Proveedor>, INotifyPropertyChanged
    {
        public ProveedorViewModel(IProveedorService proveedorService, IDialogService dialogService)
            : base(proveedorService, dialogService) {}
    }
}
