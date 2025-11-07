using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class OperarioViewModel : EntidadViewModel<Operario>
{
    public ObservableCollection<Operario> Operarios => Entidades;
    public OperarioViewModel(IOperarioService operarioService, IDialogService dialogService)
        : base(operarioService, dialogService) {}
}