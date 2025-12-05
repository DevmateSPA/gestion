using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel
{
    public class EncuadernacionViewModel : EntidadViewModel<Encuadernacion>, INotifyPropertyChanged
    {
        public ObservableCollection<Encuadernacion> Encuadernaciones => Entidades;

        private ObservableCollection<Encuadernacion> _encuadernacionesFiltradas = new();
        public ObservableCollection<Encuadernacion> EncuadernacionesFiltradas
        {
            get => _encuadernacionesFiltradas;
            set { _encuadernacionesFiltradas = value; OnPropertyChanged(); }
        }

        public EncuadernacionViewModel(IEncuadernacionService encuadernacionService, IDialogService dialogService)
            : base(encuadernacionService, dialogService)
        {}

        public override async Task LoadAll()
        {
            await base.LoadAll();
            EncuadernacionesFiltradas = new ObservableCollection<Encuadernacion>(Encuadernaciones);
        }

        public override async Task LoadAllByEmpresa()
        {
            await base.LoadAllByEmpresa();
            EncuadernacionesFiltradas = new ObservableCollection<Encuadernacion>(Encuadernaciones);
        }

        public override void Buscar()
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                EncuadernacionesFiltradas = new ObservableCollection<Encuadernacion>(Encuadernaciones);
                return;
            }

            var lower = Filtro.ToLower();

            EncuadernacionesFiltradas = new ObservableCollection<Encuadernacion>(
                Encuadernaciones.Where(e =>
                       (e.Descripcion?.ToLower().Contains(lower) ?? false)
                    || e.Codigo.ToString().Contains(lower)
                )
            );
        }
    }
}
