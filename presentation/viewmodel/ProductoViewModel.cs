using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class ProductoViewModel : EntidadViewModel<Producto>
{
    public ObservableCollection<Producto> Productos => Entidades;
    public ProductoViewModel(IProductoService productoService, IDialogService dialogService)
        : base(productoService, dialogService) {}
}