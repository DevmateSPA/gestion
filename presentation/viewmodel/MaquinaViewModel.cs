using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class MaquinaViewModel
{
    private readonly IMaquinaService _maquinaService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Maquina> Maquinas { get; set; } = new ObservableCollection<Maquina>();
    public MaquinaViewModel(IMaquinaService maquinaService, IDialogService dialogService)
    {
        _maquinaService = maquinaService;
        _dialogService = dialogService;
    }

    public async Task updateMaquina(Maquina maquina)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            await _maquinaService.Update(maquina);

            var index = Maquinas.IndexOf(Maquinas.FirstOrDefault(b => b.Id == maquina.Id));
            if (index >= 0)
                Maquinas[index] = maquina;


        }, _dialogService, "Error al actualizar la máquina");
    }

    public async Task LoadMaquinas()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _maquinaService.FindAll();
            Maquinas.Clear();
            foreach (var maquina in lista)
                Maquinas.Add(maquina);
        }, _dialogService, "Error al cargar las maquinas");
    }
}
