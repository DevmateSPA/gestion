using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class MaquinaViewModel : EntidadViewModel<Maquina>
{
    public ObservableCollection<Maquina> Maquinas => Entidades;
    public MaquinaViewModel(IMaquinaService maquinaService, IDialogService dialogService)
        : base(maquinaService, dialogService) {}
}