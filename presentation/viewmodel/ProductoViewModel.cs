using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class ProductoViewModel : EntidadViewModel<Producto>, INotifyPropertyChanged
    {
        public ProductoViewModel(IProductoService productoService, IDialogService dialogService)
            : base(productoService, dialogService) {}
    }
}
