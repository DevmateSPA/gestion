using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class FotomecanicaViewModel : EntidadViewModel<Fotomecanica>
{
    public ObservableCollection<Fotomecanica> Fotomecanica => Entidades;
    public FotomecanicaViewModel(IFotomecanicaService fotomecanicaService, IDialogService dialogService)
        : base(fotomecanicaService, dialogService) {}
}
