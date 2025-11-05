using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class OperarioViewModel
{
    private readonly IOperadorService _operadorService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Operador> Operadores { get; set; } = new ObservableCollection<Operador>();
    public OperarioViewModel(IOperadorService operadorService, IDialogService dialogService)
    {
        _operadorService = operadorService;
        _dialogService = dialogService;
    }

    public async Task updateOperador(Operador operador)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            await _operadorService.Update(operador);

            var index = Operadores.IndexOf(Operadores.FirstOrDefault(b => b.Id == operador.Id));
            if (index >= 0)
                Operadores[index] = operador;


        }, _dialogService, "Error al actualizar el operario");
    }

    public async Task LoadOperadores()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _operadorService.FindAll();
            Operadores.Clear();
            foreach (var operador in lista)
                Operadores.Add(operador);
        }, _dialogService, "Error al cargar los operarios");
    }
}
