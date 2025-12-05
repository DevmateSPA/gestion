using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.presentation.utils;

namespace Gestion.presentation.viewmodel;

public class GuiaDespachoViewModel : EntidadViewModel<GuiaDespacho>, INotifyPropertyChanged
{
    public GuiaDespachoViewModel(IGuiaDespachoService guiaDespachoService, IDialogService dialogService)
        : base(guiaDespachoService, dialogService) {}
}
