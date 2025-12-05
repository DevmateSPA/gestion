using System.Collections.ObjectModel;
using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel; 

public class OrdenTrabajoViewModel : EntidadViewModel<OrdenTrabajo>, INotifyPropertyChanged
{
    public ObservableCollection<OrdenTrabajo> OrdenesTrabajo => Entidades;

    private ObservableCollection<OrdenTrabajo> _ordenesTrabajosFiltradas = new();
    public ObservableCollection<OrdenTrabajo> OrdenesTrabajoFiltradas
    {
        get => _ordenesTrabajosFiltradas;
        set { _ordenesTrabajosFiltradas = value; OnPropertyChanged(); }
    }

    public OrdenTrabajoViewModel(IOrdenTrabajoService ordenTrabajoService, IDialogService dialogService)
        : base(ordenTrabajoService, dialogService)
    {}

    public override async Task LoadAll()
    {
        await base.LoadAll();
        OrdenesTrabajoFiltradas = new ObservableCollection<OrdenTrabajo>(OrdenesTrabajo);
    }

    public override async Task LoadAllByEmpresa()
    {
        await base.LoadAllByEmpresa();
        OrdenesTrabajoFiltradas = new ObservableCollection<OrdenTrabajo>(OrdenesTrabajo);
    }
    public override void Buscar()
    {
        if (string.IsNullOrWhiteSpace(Filtro))
        {
            OrdenesTrabajoFiltradas = new ObservableCollection<OrdenTrabajo>(OrdenesTrabajo);
            return;
        }

        var lower = Filtro.ToLower();

        OrdenesTrabajoFiltradas = new ObservableCollection<OrdenTrabajo>(
            OrdenesTrabajo.Where(f =>
                   f.Folio.ToString().Contains(lower)
                || f.RutCliente.ToLower().Contains(lower)
                || f.Fecha.ToString("dd/MM/yyyy").Contains(lower)
            )
        );
    }
}
