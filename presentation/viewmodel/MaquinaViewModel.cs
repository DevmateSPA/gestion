using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class MaquinaViewModel : EntidadViewModel<Maquina>, INotifyPropertyChanged
    {
        public ObservableCollection<Maquina> Maquinas => Entidades;

        private ObservableCollection<Maquina> _maquinasFiltradas = new();
        public ObservableCollection<Maquina> MaquinasFiltradas
        {
            get => _maquinasFiltradas;
            set { _maquinasFiltradas = value; OnPropertyChanged(); }
        }

        public MaquinaViewModel(IMaquinaService maquinaService, IDialogService dialogService)
            : base(maquinaService, dialogService)
        {}

        public override async Task LoadAll()
        {
            await base.LoadAll();
            MaquinasFiltradas = new ObservableCollection<Maquina>(Maquinas);
        }

        public override async Task LoadAllByEmpresa()
        {
            await base.LoadAllByEmpresa();
            MaquinasFiltradas = new ObservableCollection<Maquina>(Maquinas);
        }
    }
}
