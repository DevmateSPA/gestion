using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class OrdenTrabajoPeliculaViewModel : EntidadViewModel<OrdenTrabajoPelicula>
{
    public ObservableCollection<OrdenTrabajoPelicula> OrdenTrabajoPelicula => Entidades;
    public OrdenTrabajoPeliculaViewModel(IOrdenTrabajoPeliculaService ordenTrabajoPeliculaService, IDialogService dialogService)
        : base(ordenTrabajoPeliculaService, dialogService) {}
}
