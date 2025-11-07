using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class GrupoViewModel : EntidadViewModel<Grupo>
{
    public ObservableCollection<Grupo> Grupos => Entidades;
    public GrupoViewModel(IGrupoService grupoService, IDialogService dialogService)
        : base(grupoService, dialogService) {}
}