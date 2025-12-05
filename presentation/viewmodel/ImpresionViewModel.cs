using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel
{
    public class ImpresionViewModel : EntidadViewModel<Impresion>, INotifyPropertyChanged
    {
        public ObservableCollection<Impresion> Impresiones => Entidades;

        private ObservableCollection<Impresion> _impresionesFiltradas = new();
        public ObservableCollection<Impresion> ImpresionesFiltradas
        {
            get => _impresionesFiltradas;
            set { _impresionesFiltradas = value; OnPropertyChanged(); }
        }

        public ImpresionViewModel(IImpresionService impresionService, IDialogService dialogService)
            : base(impresionService, dialogService)
        {}

        public override async Task LoadAll()
        {
            await base.LoadAll();
            ImpresionesFiltradas = new ObservableCollection<Impresion>(Impresiones);
        }

        public override async Task LoadAllByEmpresa()
        {
            await base.LoadAllByEmpresa();
            ImpresionesFiltradas = new ObservableCollection<Impresion>(Impresiones);
        }

        public void Buscar()
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                ImpresionesFiltradas = new ObservableCollection<Impresion>(Impresiones);
                return;
            }

            var lower = Filtro.ToLower();

            ImpresionesFiltradas = new ObservableCollection<Impresion>(
                Impresiones.Where(i =>
                       (i.Descripcion?.ToLower().Contains(lower) ?? false)
                    || i.Codigo.ToString().Contains(lower)
                )
            );
        }
    }
}
