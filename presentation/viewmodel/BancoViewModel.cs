using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class BancoViewModel
{
    private readonly IBancoService _bancoService;
    public ObservableCollection<Banco> Bancos { get; set; } = new ObservableCollection<Banco>();
    public BancoViewModel(IBancoService bancoService)
    {
        _bancoService = bancoService;
    }

    public async Task LoadBancos()
    {
        var lista = await _bancoService.FindAll();
        Bancos.Clear();
        foreach (var banco in lista)
            Bancos.Add(banco);
    }
}
