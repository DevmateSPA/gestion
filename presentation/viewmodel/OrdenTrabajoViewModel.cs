using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.session;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class OrdenTrabajoViewModel : EntidadViewModel<OrdenTrabajo>
{
    public ObservableCollection<OrdenTrabajo> OrdenesTrabajo => Entidades;

    private ObservableCollection<OrdenTrabajo> _ordenesTrabajosFiltradas = new();
    public ObservableCollection<OrdenTrabajo> OrdenesTrabajoFiltradas
    {
        get => _ordenesTrabajosFiltradas;
        set { _ordenesTrabajosFiltradas = value; OnPropertyChanged(); }
    }

    private string _filtro = "";
    public string Filtro
    {
        get => _filtro;
        set { _filtro = value; OnPropertyChanged(); }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public OrdenTrabajoViewModel(IOrdenTrabajoService ordenTrabajoService, IDialogService dialogService)
        : base(ordenTrabajoService, dialogService)
    {}

    public override async Task LoadAll()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            long empresaId = SesionApp.IdEmpresa;

            var servicio = (IOrdenTrabajoService)_service;
            var lista = await servicio.FindAllByEmpresa(empresaId);

            if (!lista.Any())
            {
                _dialogService.ShowMessage(
                    $"No existen Ordenes de Trabajo para la empresa: {SesionApp.NombreEmpresa}.",
                    "Información");
            }

            var dateProp = GetDateProperty(typeof(OrdenTrabajo));
            if (dateProp != null)
            {
                lista = lista.OrderByDescending(x => dateProp.GetValue(x)).ToList();
            }

            Entidades.Clear();
            foreach (var entidad in lista)
                addEntity(entidad);

            OrdenesTrabajoFiltradas = new ObservableCollection<OrdenTrabajo>(Entidades);
            MessageBox("ordenadas: "+OrdenesTrabajoFiltradas);

        }, _dialogService, "Error al cargar Ordenes de Trabajo");
    }

    public void Buscar()
    {
        if (string.IsNullOrWhiteSpace(Filtro))
        {
            OrdenesTrabajoFiltradas = new ObservableCollection<OrdenTrabajo>(OrdenesTrabajo);
            return;
        }

        var lower = Filtro.ToLower();

        OrdenesTrabajoFiltradas = new ObservableCollection<OrdenTrabajo>(
            OrdenesTrabajo.Where(f =>
                   f.Folio.ToString().Contains(lower)
                || f.RutCliente.ToLower().Contains(lower)
                || f.Fecha.ToString("dd/MM/yyyy").Contains(lower)
            )
        );
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
