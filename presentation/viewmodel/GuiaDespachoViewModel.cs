using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.presentation.utils;

namespace Gestion.presentation.viewmodel;

public class GuiaDespachoViewModel : EntidadViewModel<GuiaDespacho>, INotifyPropertyChanged
{
    public ObservableCollection<GuiaDespacho> GuiasDespacho => Entidades;

    private ObservableCollection<GuiaDespacho> _guiasFiltradas = new();
    public ObservableCollection<GuiaDespacho> GuiasDespachoFiltradas
    {
        get => _guiasFiltradas;
        set { _guiasFiltradas = value; OnPropertyChanged(); }
    }

    public GuiaDespachoViewModel(IGuiaDespachoService guiaDespachoService, IDialogService dialogService)
        : base(guiaDespachoService, dialogService)
    {}

    public override async Task LoadAll()
    {
        await base.LoadAll();
        GuiasDespachoFiltradas = new ObservableCollection<GuiaDespacho>(GuiasDespacho);
    }

    public override async Task LoadAllByEmpresa()
    {
        await base.LoadAllByEmpresa();
        GuiasDespachoFiltradas = new ObservableCollection<GuiaDespacho>(GuiasDespacho);
    }
}
