using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class EncuadernacionViewModel : EntidadViewModel<Encuadernacion>
{
    public ObservableCollection<Encuadernacion> Encuadernacion => Entidades;
    public EncuadernacionViewModel(IEncuadernacionService encuadernacionService, IDialogService dialogService)
        : base(encuadernacionService, dialogService) {}
}