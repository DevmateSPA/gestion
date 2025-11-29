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

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public FacturaViewModel(IFacturaService facturaService, IDialogService dialogService)
        : base(facturaService, dialogService) {}


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
