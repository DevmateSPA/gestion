using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class OperarioViewModel
{
    private readonly IOperadorService _operadorService;
    public ObservableCollection<Operador> Operadores { get; set; } = new ObservableCollection<Operador>();
    public OperarioViewModel(IOperadorService operadorService)
    {
        _operadorService = operadorService;
    }

    public async Task LoadOperadores()
    {
        var lista = await _operadorService.FindAll();
        Operadores.Clear();
        foreach (var operador in lista)
            Operadores.Add(operador);
    }
}
