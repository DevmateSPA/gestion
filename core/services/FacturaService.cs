using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class FacturaService : BaseService<Factura>, IFacturaService
{
    private readonly IFacturaRepository _facturaRepository;
    public FacturaService(IFacturaRepository facturaRepository)
        : base(facturaRepository)
    {
        _facturaRepository = facturaRepository;
    }

    public async Task<List<Factura>> FindAllByRutBetweenFecha(long empresaId, string rutCliente, DateTime fechaDesde, DateTime fechaHasta)
    {
        return await _facturaRepository.FindAllByRutBetweenFecha(empresaId, rutCliente, fechaDesde, fechaHasta);
    }

    public async Task<List<string>> GetFolioList(string input, long empresaId)
    {
        return await _facturaRepository.GetFolioList(
            input,
            empresaId);
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        Factura entity,
        long? excludeId = null)
    {
        List<string> erroresEncontrados = [];

        if (await _facturaRepository.ExisteFolio(
                folio: entity.Folio,
                empresaId: entity.Empresa,
                excludeId: excludeId))
            erroresEncontrados.Add($"El folio de la factura: {entity.Folio}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Folio))
            erroresEncontrados.Add("El folio de la factura es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.RutCliente))
            erroresEncontrados.Add("El rut del cliente de la factura es obligatorio.");

        if (entity.FechaVencimiento.Date < entity.Fecha.Date)
            erroresEncontrados.Add("La fecha de vencimiento no puede ser anterior a la fecha de la factura.");

        return erroresEncontrados;
    }
}