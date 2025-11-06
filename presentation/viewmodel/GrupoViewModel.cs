using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class GrupoViewModel
{
    private readonly IGrupoService _grupoService;
    private readonly IDialogService _dialogService;
    public ObservableCollection<Grupo> Grupos { get; set; } = new ObservableCollection<Grupo>();
    public GrupoViewModel(IGrupoService grupoService, IDialogService dialogService)
    {
        _grupoService = grupoService;
        _dialogService = dialogService;
    }

    public async Task updateGrupo(Grupo grupo)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            await _grupoService.Update(grupo);

            var index = Grupos.IndexOf(Grupos.FirstOrDefault(b => b.Id == grupo.Id));
            if (index >= 0)
                Grupos[index] = grupo;


        }, _dialogService, "Error al actualizar el grupo");
    }

    public async Task LoadGrupos()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _grupoService.FindAll();
            Grupos.Clear();
            foreach (var grupo in lista)
                Grupos.Add(grupo);
        }, _dialogService, "Error al cargar los grupos");
    }
}
