using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class GuiaDespachoViewModel : EntidadViewModel<GuiaDespacho>
{
    public ObservableCollection<GuiaDespacho> GuiasDespacho => Entidades;
    public GuiaDespachoViewModel(IGuiaDespachoService guiaDespachoService, IDialogService dialogService)
        : base(guiaDespachoService, dialogService) {}
}
