using System.Collections.ObjectModel;
using System.ComponentModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;
public class UsuarioViewModel : EntidadViewModel<Usuario>, INotifyPropertyChanged
{
    public UsuarioViewModel(IUsuarioService usuarioService, IDialogService dialogService)
        : base(usuarioService, dialogService) {}
}
