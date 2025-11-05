using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class EncuadernacionViewModel
{
    private readonly IEncuadernacionService _encuadernacionService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Encuadernacion> Encuadernacion { get; set; } = new ObservableCollection<Encuadernacion>();
    public EncuadernacionViewModel(IEncuadernacionService encuadernacionService, IDialogService dialogService)
    {
        _encuadernacionService = encuadernacionService;
        _dialogService = dialogService;
    }

    public async Task updateEncuadernacion(Encuadernacion encuadernacion)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            await _encuadernacionService.Update(encuadernacion);

            var index = Encuadernacion.IndexOf(Encuadernacion.FirstOrDefault(b => b.Id == encuadernacion.Id));
            if (index >= 0)
                Encuadernacion[index] = encuadernacion;


        }, _dialogService, "Error al actualizar la encuadernación");
    }

    public async Task LoadEncuadernaciones()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _encuadernacionService.FindAll();
            Encuadernacion.Clear();
            foreach (var encuadernacion in lista)
                Encuadernacion.Add(encuadernacion);
        }, _dialogService, "Error al cargar las encuadernaciones");
    }
}
