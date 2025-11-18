using System.Collections.ObjectModel;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.helpers;

namespace Gestion.presentation.viewmodel;

public class FacturaViewModel : EntidadViewModel<Factura>
{
    private readonly IFacturaService _facturaService;
    public ObservableCollection<Factura> Facturas => Entidades;
    public FacturaViewModel(IFacturaService facturaService, IDialogService dialogService)
        : base(facturaService, dialogService)
    {
        _facturaService = facturaService;
    }

    public override async Task LoadAll()
    {
        await SafeExecutor.RunAsync(async () =>
        {
            var lista = await _facturaService.FindAllWithDetails();
            Facturas.Clear();
            foreach (var entidad in lista)
                addEntity(entidad);
        }, _dialogService, $"Error al cargar las facturas");
    }
}
