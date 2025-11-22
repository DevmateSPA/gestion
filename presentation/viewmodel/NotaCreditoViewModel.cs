using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class NotaCreditoViewModel : EntidadViewModel<NotaCredito>, INotifyPropertyChanged
{
    public ObservableCollection<NotaCredito> NotasCredito => Entidades;

    private ObservableCollection<NotaCredito> _notasFiltradas = new();
    public ObservableCollection<NotaCredito> NotasCreditoFiltradas
    {
        get => _notasFiltradas;
        set { _notasFiltradas = value; OnPropertyChanged(); }
    }

    private string _filtro = "";
    public string Filtro
    {
        get => _filtro;
        set { _filtro = value; OnPropertyChanged(); }
    }

    public NotaCreditoViewModel(INotaCreditoService notaCreditoService, IDialogService dialogService)
        : base(notaCreditoService, dialogService)
    {}

    public override async Task LoadAll()
    {
        await base.LoadAll();
        NotasCreditoFiltradas = new ObservableCollection<NotaCredito>(NotasCredito);
    }

    public void Buscar()
    {
        if (string.IsNullOrWhiteSpace(Filtro))
        {
            NotasCreditoFiltradas = new ObservableCollection<NotaCredito>(NotasCredito);
            return;
        }

        var lower = Filtro.ToLower();

        NotasCreditoFiltradas = new ObservableCollection<NotaCredito>(
            NotasCredito.Where(n =>
                   n.Folio.ToString().Contains(lower)
                || (n.RutCliente?.ToLower().Contains(lower) ?? false)
                || n.Fecha.ToString("dd/MM/yyyy").Contains(lower)
            )
        );
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
