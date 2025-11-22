using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel
{
    public class OperarioViewModel : EntidadViewModel<Operario>, INotifyPropertyChanged
    {
        public ObservableCollection<Operario> Operarios => Entidades;

        private ObservableCollection<Operario> _operariosFiltrados = new();
        public ObservableCollection<Operario> OperariosFiltrados
        {
            get => _operariosFiltrados;
            set { _operariosFiltrados = value; OnPropertyChanged(); }
        }

        private string _filtro = "";
        public string Filtro
        {
            get => _filtro;
            set { _filtro = value; OnPropertyChanged(); }
        }

        public OperarioViewModel(IOperarioService operarioService, IDialogService dialogService)
            : base(operarioService, dialogService)
        {}

        public override async Task LoadAll()
        {
            await base.LoadAll();
            OperariosFiltrados = new ObservableCollection<Operario>(Operarios);
        }

        public void Buscar()
        {
            if (string.IsNullOrWhiteSpace(Filtro))
            {
                OperariosFiltrados = new ObservableCollection<Operario>(Operarios);
                return;
            }

            var lower = Filtro.ToLower();

            OperariosFiltrados = new ObservableCollection<Operario>(
                Operarios.Where(o =>
                       (o.Nombre?.ToLower().Contains(lower) ?? false)
                    || (o.Codigo?.ToLower().Contains(lower) ?? false)
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
