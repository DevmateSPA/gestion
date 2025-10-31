using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class MaquinaViewModel
{
    private readonly IMaquinaService _maquinaService;
    public ObservableCollection<Maquina> Maquinas { get; set; } = new ObservableCollection<Maquina>();
    public MaquinaViewModel(IMaquinaService maquinaService)
    {
        _maquinaService = maquinaService;
    }

    public async Task LoadMaquinas()
    {
        var lista = await _maquinaService.FindAll();
        Maquinas.Clear();
        foreach (var maquina in lista)
            Maquinas.Add(maquina);
    }
}
