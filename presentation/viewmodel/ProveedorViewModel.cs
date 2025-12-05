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
        public ObservableCollection<Proveedor> Proveedores => Entidades;

        private ObservableCollection<Proveedor> _proveedoresFiltrados = new();
        public ObservableCollection<Proveedor> ProveedoresFiltrados
        {
            get => _proveedoresFiltrados;
            set { _proveedoresFiltrados = value; OnPropertyChanged(); }
        }

        public ProveedorViewModel(IProveedorService proveedorService, IDialogService dialogService)
            : base(proveedorService, dialogService)
        {}

        public override async Task LoadAll()
        {
            await base.LoadAll();
            ProveedoresFiltrados = new ObservableCollection<Proveedor>(Proveedores);
        }

        public override async Task LoadAllByEmpresa()
        {
            await base.LoadAllByEmpresa();
            ProveedoresFiltrados = new ObservableCollection<Proveedor>(Proveedores);
        }
    }
}
