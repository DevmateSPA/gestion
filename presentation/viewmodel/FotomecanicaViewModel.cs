using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel
{
    public class FotomecanicaViewModel : EntidadViewModel<Fotomecanica>, INotifyPropertyChanged
    {
        public ObservableCollection<Fotomecanica> Fotomecanicas => Entidades;

        private ObservableCollection<Fotomecanica> _fotomecanicasFiltradas = new();
        public ObservableCollection<Fotomecanica> FotomecanicasFiltradas
        {
            get => _fotomecanicasFiltradas;
            set { _fotomecanicasFiltradas = value; OnPropertyChanged(); }
        }

        public FotomecanicaViewModel(IFotomecanicaService fotomecanicaService, IDialogService dialogService)
            : base(fotomecanicaService, dialogService)
        {}

        public override async Task LoadAll()
        {
            await base.LoadAll();
            FotomecanicasFiltradas = new ObservableCollection<Fotomecanica>(Fotomecanicas);
        }

        public override async Task LoadAllByEmpresa()
        {
            await base.LoadAllByEmpresa();
            FotomecanicasFiltradas = new ObservableCollection<Fotomecanica>(Fotomecanicas);
        }

        public void Buscar()
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                FotomecanicasFiltradas = new ObservableCollection<Fotomecanica>(Fotomecanicas);
                return;
            }

            var lower = Filtro.ToLower();

            FotomecanicasFiltradas = new ObservableCollection<Fotomecanica>(
                Fotomecanicas.Where(f =>
                       (f.Descripcion?.ToLower().Contains(lower) ?? false)
                    || f.Codigo.ToString().Contains(lower)
                )
            );
        }
    }
}
