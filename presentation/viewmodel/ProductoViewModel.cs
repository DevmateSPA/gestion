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
        public ObservableCollection<Producto> Productos => Entidades;

        private ObservableCollection<Producto> _productosFiltrados = new();
        public ObservableCollection<Producto> ProductosFiltrados
        {
            get => _productosFiltrados;
            set { _productosFiltrados = value; OnPropertyChanged(); }
        }

        public ProductoViewModel(IProductoService productoService, IDialogService dialogService)
            : base(productoService, dialogService)
        {}

        public override async Task LoadAll()
        {
            await base.LoadAll();
            ProductosFiltrados = new ObservableCollection<Producto>(Productos);
        }

        public override async Task LoadAllByEmpresa()
        {
            await base.LoadAllByEmpresa();
            ProductosFiltrados = new ObservableCollection<Producto>(Productos);
        }

        public void Buscar()
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                ProductosFiltrados = new ObservableCollection<Producto>(Productos);
                return;
            }

            var lower = Filtro.ToLower();

            ProductosFiltrados = new ObservableCollection<Producto>(
                Productos.Where(p =>
                       (p.Descripcion?.ToLower().Contains(lower) ?? false)
                    || p.Codigo.ToString().Contains(lower)
                )
            );
        }
    }
}
