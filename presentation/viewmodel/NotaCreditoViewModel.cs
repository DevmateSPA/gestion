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

    public NotaCreditoViewModel(INotaCreditoService notaCreditoService, IDialogService dialogService)
        : base(notaCreditoService, dialogService)
    {}

    public override async Task LoadAll()
    {
        await base.LoadAll();
        NotasCreditoFiltradas = new ObservableCollection<NotaCredito>(NotasCredito);
    }

    public override async Task LoadAllByEmpresa()
    {
        await base.LoadAllByEmpresa();
        NotasCreditoFiltradas = new ObservableCollection<NotaCredito>(NotasCredito);
    }
}
