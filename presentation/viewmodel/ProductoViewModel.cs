using System.ComponentModel;
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
