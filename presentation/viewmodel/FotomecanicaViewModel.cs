using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class FotomecanicaViewModel
{
    private readonly IFotomecanicaService _fotomecanicaService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Fotomecanica> Fotomecanica { get; set; } = new ObservableCollection<Fotomecanica>();
    public FotomecanicaViewModel(IFotomecanicaService fotomecanicaService, IDialogService dialogService)
    {
        _fotomecanicaService = fotomecanicaService;
        _dialogService = dialogService;
    }

    public async Task updateFotomecanica(Fotomecanica fotomecanica)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            await _fotomecanicaService.Update(fotomecanica);

            var index = Fotomecanica.IndexOf(Fotomecanica.FirstOrDefault(b => b.Id == fotomecanica.Id));
            if (index >= 0)
                Fotomecanica[index] = fotomecanica;


        }, _dialogService, "Error al actualizar la fotomecánica");
    }
    
    public async Task LoadFotomecanica()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _fotomecanicaService.FindAll();
            Fotomecanica.Clear();
            foreach (var fotomecanica in lista)
                Fotomecanica.Add(fotomecanica);
        }, _dialogService, "Error al cargar la fotomecanica");
    }
}
