using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class FacturaCompraService : BaseService<FacturaCompra>, IFacturaCompraService
{
    private readonly IFacturaCompraRepository _facturaCompraRepository;
    public FacturaCompraService(IFacturaCompraRepository facturaCompraRepository)
        :base(facturaCompraRepository)
    {
        _facturaCompraRepository = facturaCompraRepository;
    }

    protected override IEnumerable<IReglaNegocio<FacturaCompra>> DefinirReglas(
        FacturaCompra entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<FacturaCompra>(
                f => f.Folio,
                "El folio de la factura es obligatorio."),

            new RequeridoRegla<FacturaCompra>(
                f => f.RutCliente,
                "El rut del cliente de la factura es obligatorio."),

            new NoAnteriorFechaRegla<FacturaCompra>(
                f => f.FechaVencimiento,
                f => f.Fecha,
                "La fecha de vencimiento no puede ser anterior a la fecha de la factura."),

            new UnicoRegla<FacturaCompra>(
                existe: (f, id) =>
                    _facturaCompraRepository.ExisteFolio(
                        f.Folio,
                        f.Empresa,
                        id),

                valor: f => f.Folio,

                mensaje: "El folio de la factura: {0}, ya existe para la empresa actual.")
        ];
    }
}