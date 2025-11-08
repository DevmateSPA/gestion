using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.presentation.viewmodel;

public class OrdenCompraPeliculaViewModel : EntidadViewModel<OrdenCompraPelicula>
{
    public ObservableCollection<OrdenCompraPelicula> OrdenCompraPelicula => Entidades;
    public OrdenCompraPeliculaViewModel(IOrdenCompraPeliculaService ordenCompraPeliculaService, IDialogService dialogService)
        : base(ordenCompraPeliculaService, dialogService) {}
}
