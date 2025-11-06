using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class OperarioViewModel
{
    private readonly IOperarioService _operarioService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Operario> Operarios { get; set; } = new ObservableCollection<Operario>();
    public OperarioViewModel(IOperarioService operadorService, IDialogService dialogService)
    {
        _operarioService = operadorService;
        _dialogService = dialogService;
    }

    public async Task updateOperario(Operario operario)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            await _operarioService.Update(operario);

            var index = Operarios.IndexOf(Operarios.FirstOrDefault(b => b.Id == operario.Id));
            if (index >= 0)
                Operarios[index] = operario;


        }, _dialogService, "Error al actualizar el operario");
    }

    public async Task LoadOperarios()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _operarioService.FindAll();
            Operarios.Clear();
            foreach (var operador in lista)
                Operarios.Add(operador);
        }, _dialogService, "Error al cargar los operarios");
    }
}
