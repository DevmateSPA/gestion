using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class ImpresionViewModel : EntidadViewModel<Impresion>
{
    public ObservableCollection<Impresion> Impresion => Entidades;
    public ImpresionViewModel(IImpresionService impresionService, IDialogService dialogService)
        : base(impresionService, dialogService) {}
}