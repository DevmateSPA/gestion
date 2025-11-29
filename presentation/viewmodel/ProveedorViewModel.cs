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

        private string _filtro = "";
        public string Filtro
        {
            get => _filtro;
            set { _filtro = value; OnPropertyChanged(); }
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

        public void Buscar()
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                ProveedoresFiltrados = new ObservableCollection<Proveedor>(Proveedores);
                return;
            }

            var lower = Filtro.ToLower();

            ProveedoresFiltrados = new ObservableCollection<Proveedor>(
                Proveedores.Where(p =>
                       (p.Razon_Social?.ToLower().Contains(lower) ?? false)
                    || (p.Rut?.ToLower().Contains(lower) ?? false)
                    || (p.Giro?.ToLower().Contains(lower) ?? false)
                )
            );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
