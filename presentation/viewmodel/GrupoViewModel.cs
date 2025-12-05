using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class GrupoViewModel : EntidadViewModel<Grupo>, INotifyPropertyChanged
    {
        public ObservableCollection<Grupo> Grupos => Entidades;

        private ObservableCollection<Grupo> _gruposFiltrados = new();
        public ObservableCollection<Grupo> GruposFiltrados
        {
            get => _gruposFiltrados;
            set { _gruposFiltrados = value; OnPropertyChanged(); }
        }

        public GrupoViewModel(IGrupoService grupoService, IDialogService dialogService)
            : base(grupoService, dialogService)
        {}

        public override async Task LoadAll()
        {
            await base.LoadAll();
            GruposFiltrados = new ObservableCollection<Grupo>(Grupos);
        }

        public override async Task LoadAllByEmpresa()
        {
            await base.LoadAllByEmpresa();
            GruposFiltrados = new ObservableCollection<Grupo>(Grupos);
        }

        public override void Buscar()
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                GruposFiltrados = new ObservableCollection<Grupo>(Grupos);
                return;
            }

            var lower = Filtro.ToLower();

            GruposFiltrados = new ObservableCollection<Grupo>(
                Grupos.Where(g =>
                       g.Codigo.ToString().Contains(lower)
                    || (g.Descripcion?.ToLower().Contains(lower) ?? false)
                )
            );
        }
    }
}
