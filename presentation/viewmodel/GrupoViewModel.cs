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

    public async Task update(Grupo grupo)
    {
        await SafeExecutor.RunAsync(async () =>
        {
            if (await _grupoService.Update(grupo))
            {
                var existing = Grupos.FirstOrDefault(g => g.Id == grupo.Id);
                if (existing != null)
                {
                    var index = Grupos.IndexOf(existing);
                    if (index >= 0)
                        Grupos[index] = grupo;
                }
            }
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
