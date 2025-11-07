using System.Collections.ObjectModel;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class ProveedorViewModel : EntidadViewModel<Proveedor>
{
    public ObservableCollection<Proveedor> Proveedores => Entidades;
    public ProveedorViewModel(IProveedorService proveedorService, IDialogService dialogService)
        : base(proveedorService, dialogService) {}
}