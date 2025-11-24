using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.helpers;
using Gestion.presentation.utils;

namespace Gestion.presentation.viewmodel;

public class FacturaViewModel : EntidadViewModel<Factura>, INotifyPropertyChanged
{
    public ObservableCollection<Factura> Facturas => Entidades;

    private ObservableCollection<Factura> _facturasFiltradas = new();
    public ObservableCollection<Factura> FacturasFiltradas
    {
        get => _facturasFiltradas;
        set { _facturasFiltradas = value; OnPropertyChanged(); }
    }

    private string _filtro = "";
    public string Filtro
    {
        get => _filtro;
        set { _filtro = value; OnPropertyChanged(); }
    }

    public FacturaViewModel(IFacturaService facturaService, IDialogService dialogService)
        : base(facturaService, dialogService)
    {}

    public override async Task LoadAll()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            long empresaId = SesionApp.IdEmpresa;

            var servicio = (IFacturaService)_service;
            var lista = await servicio.FindAllByEmpresa(empresaId);


            if (!lista.Any())
            {
                _dialogService.ShowMessage(
                    $"No existen facturas para la empresa: {SesionApp.NombreEmpresa}.",
                    "Información");
            }

            var dateProp = GetDateProperty(typeof(Factura));
            if (dateProp != null)
            {
                lista = lista.OrderByDescending(x => dateProp.GetValue(x)).ToList();
            }

            Entidades.Clear();
            foreach (var entidad in lista)
                addEntity(entidad);

            FacturasFiltradas = new ObservableCollection<Factura>(Entidades);

        }, _dialogService, "Error al cargar facturas");
    }

    public void Buscar()
    {
        if (string.IsNullOrWhiteSpace(Filtro))
        {
            FacturasFiltradas = new ObservableCollection<Factura>(Facturas);
            return;
        }

        var lower = Filtro.ToLower();

        FacturasFiltradas = new ObservableCollection<Factura>(
            Facturas.Where(f =>
                   f.Folio.ToString().Contains(lower)
                || f.RutCliente.ToLower().Contains(lower)
                || f.Fecha.ToString("dd/MM/yyyy").Contains(lower)
            )
        );
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
