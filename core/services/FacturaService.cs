using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

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

    protected override IEnumerable<IReglaNegocio<Factura>> DefinirReglas(
        Factura entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Factura>(
                f => f.Folio,
                "El folio de la factura es obligatorio."),

            new RequeridoRegla<Factura>(
                f => f.RutCliente,
                "El rut del cliente de la factura es obligatorio."),

            new NoAnteriorFechaRegla<Factura>(
                f => f.FechaVencimiento,
                f => f.Fecha,
                "La fecha de vencimiento no puede ser anterior a la fecha de la factura."),

            new UnicoRegla<Factura>(
                existe: (f, id) =>
                    _facturaRepository.ExisteFolio(
                        f.Folio,
                        f.Empresa,
                        id),

                valor: f => f.Folio,

                mensaje: "El folio de la factura: {0}, ya existe para la empresa actual.")
        ];
    }

    public async Task<string> GetSiguienteFolio(long empresaId)
    {
        return await _facturaRepository.GetSiguienteFolio(empresaId);
    }
}