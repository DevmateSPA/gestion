using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class EncuadernacionViewModel
{
    private readonly IEncuadernacionService _encuadernacionService;
    public ObservableCollection<Encuadernacion> Encuadernacion { get; set; } = new ObservableCollection<Encuadernacion>();
    public EncuadernacionViewModel(IEncuadernacionService encuadernacionService)
    {
        _encuadernacionService = encuadernacionService;
    }
    
    public async Task LoadEncuadernaciones()
    {
        var lista = await _encuadernacionService.FindAll();
        Encuadernacion.Clear();
        foreach (var encuadernacion in lista)
            Encuadernacion.Add(encuadernacion);
    }
}
