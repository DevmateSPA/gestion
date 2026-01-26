using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class NotaCreditoService : BaseService<NotaCredito>, INotaCreditoService
{
    private readonly INotaCreditoRepository _notaCreditoRepository;
    public NotaCreditoService(INotaCreditoRepository notaCreditoRepository)
        :base(notaCreditoRepository)
    {
        _notaCreditoRepository = notaCreditoRepository;
    }

    protected override IEnumerable<IReglaNegocio<NotaCredito>> DefinirReglas(
        NotaCredito entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<NotaCredito>(
                c => c.Folio,
                "El folio de la nota de crédito es obligatorio."),

            new RequeridoRegla<NotaCredito>(
                c => c.RutCliente,
                "El rut del cliente de la nota de crédito es obligatorio."),

            new UnicoRegla<NotaCredito>(
                existe: (nc, id) =>
                    _notaCreditoRepository.ExisteFolio(
                        nc.Folio,
                        nc.Empresa,
                        id),

                valor: nc => nc.Folio,

                mensaje: "El folio de la nota de crédito: {0}, ya existe para la empresa actual.")
        ];
    }

    public async Task<string> GetSiguienteFolio(long empresaId)
    {
        return await _notaCreditoRepository.GetSiguienteFolio(empresaId);
    }
}