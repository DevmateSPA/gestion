using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class ImpresionViewModel
{
    private readonly IImpresionService _impresionService;
    public ObservableCollection<Impresion> Impresion { get; set; } = new ObservableCollection<Impresion>();
    public ImpresionViewModel(IImpresionService impresionService)
    {
        _impresionService = impresionService;
    }
    
    public async Task LoadImpresion()
    {
        var lista = await _impresionService.FindAll();
        Impresion.Clear();
        foreach (var impresion in lista)
            Impresion.Add(impresion);
    }
}
