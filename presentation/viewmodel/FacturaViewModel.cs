using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
namespace Gestion.presentation.viewmodel;

public class FacturaViewModel : EntidadViewModel<Factura>
{
    public ObservableCollection<Factura> Facturas => Entidades;

    private ObservableCollection<Factura> _facturasFiltradas = new();
    public ObservableCollection<Factura> FacturasFiltradas
    {
        get => _facturasFiltradas;
        set { _facturasFiltradas = value; OnPropertyChanged(); }
    }

    public FacturaViewModel(IFacturaService facturaService, IDialogService dialogService)
        : base(facturaService, dialogService) {}


    public override async Task LoadAll()
    {
        await base.LoadAll();
        FacturasFiltradas = new ObservableCollection<Factura>(Facturas);
    }

    public override async Task LoadAllByEmpresa()
    {
        await base.LoadAllByEmpresa();
        FacturasFiltradas = new ObservableCollection<Factura>(Facturas);
    }

    public override void Buscar()
    {
        if (string.IsNullOrWhiteSpace(Filtro))
        {
            FacturasFiltradas = new ObservableCollection<Factura>(Facturas);
            return;
        }

        var lower = Filtro.ToLower();

        FacturasFiltradas = new ObservableCollection<Factura>(
            Facturas.Where(f =>
                   f.Folio.ToLower().ToString().Contains(lower)
                || f.RutCliente.ToLower().Contains(lower)
                || f.Fecha.ToString("dd/MM/yyyy").Contains(lower)
            )
        );
    }
}
