using System.Collections.ObjectModel;
using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;
public class BancoViewModel : EntidadViewModel<Banco>, INotifyPropertyChanged
{
    public ObservableCollection<Banco> Bancos => Entidades;

    private ObservableCollection<Banco> _bancosFiltrados = new();
    public ObservableCollection<Banco> BancosFiltrados
    {
        get => _bancosFiltrados;
        set { _bancosFiltrados = value; OnPropertyChanged(); }
    }

    public BancoViewModel(IBancoService bancoService, IDialogService dialogService)
        : base(bancoService, dialogService) {}

    public override async Task LoadAll()
    {
        await base.LoadAll();
        BancosFiltrados = new ObservableCollection<Banco>(Bancos);
    }

    public override async Task LoadAllByEmpresa()
    {
        await base.LoadAllByEmpresa();
        BancosFiltrados = new ObservableCollection<Banco>(Bancos);
    }

    public void Buscar()
    {
        if (string.IsNullOrWhiteSpace(Filtro))
        {
            BancosFiltrados = new ObservableCollection<Banco>(Bancos);
            return;
        }

        var lower = Filtro.ToLower();

        BancosFiltrados = new ObservableCollection<Banco>(
            Bancos.Where(b =>
                (b.Nombre?.ToLower().Contains(lower) ?? false)
                || (b.Codigo?.ToLower().Contains(lower) ?? false)
            )
        );
    }
}
