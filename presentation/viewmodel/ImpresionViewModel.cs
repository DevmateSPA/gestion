using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class ImpresionViewModel
{
    private readonly IImpresionService _impresionService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Impresion> Impresion { get; set; } = new ObservableCollection<Impresion>();
    public ImpresionViewModel(IImpresionService impresionService, IDialogService dialogService)
    {
        _impresionService = impresionService;
        _dialogService = dialogService;
    }

    public async Task updateImpresion(Impresion impresion)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            await _impresionService.Update(impresion);

            var index = Impresion.IndexOf(Impresion.FirstOrDefault(b => b.Id == impresion.Id));
            if (index >= 0)
                Impresion[index] = impresion;


        }, _dialogService, "Error al actualizar la impresión");
    }
    
    public async Task LoadImpresion()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _impresionService.FindAll();
            Impresion.Clear();
            foreach (var impresion in lista)
                Impresion.Add(impresion);
        }, _dialogService, "Error al cargar las impresiones");
    }
}
