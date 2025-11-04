using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class FotomecanicaViewModel
{
    private readonly IFotomecanicaService _fotomecanicaService;
    public ObservableCollection<Fotomecanica> Fotomecanica { get; set; } = new ObservableCollection<Fotomecanica>();
    public FotomecanicaViewModel(IFotomecanicaService fotomecanicaService)
    {
        _fotomecanicaService = fotomecanicaService;
    }
    
    public async Task LoadFotomecanica()
    {
        var lista = await _fotomecanicaService.FindAll();
        Fotomecanica.Clear();
        foreach (var fotomecanica in lista)
            Fotomecanica.Add(fotomecanica);
    }
}
