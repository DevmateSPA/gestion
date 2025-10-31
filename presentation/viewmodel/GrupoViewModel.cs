using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class GrupoViewModel
{
    private readonly IGrupoService _grupoService;
    public ObservableCollection<Grupo> Grupos { get; set; } = new ObservableCollection<Grupo>();
    public GrupoViewModel(IGrupoService grupoService)
    {
        _grupoService = grupoService;
    }

    public async Task LoadGrupos()
    {
        var lista = await _grupoService.FindAll();
        Grupos.Clear();
        foreach (var grupo in lista)
            Grupos.Add(grupo);
    }
}
