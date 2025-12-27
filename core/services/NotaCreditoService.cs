using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class NotaCreditoService : BaseService<NotaCredito>, INotaCreditoService
{
    private readonly INotaCreditoRepository _notaCreditoRepository;
    public NotaCreditoService(INotaCreditoRepository notaCreditoRepository)
        :base(notaCreditoRepository)
    {
        _notaCreditoRepository = notaCreditoRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        NotaCredito entity,
        long? excludeId = null)
    {
        List<string> erroresEncontrados = [];

        if (await _notaCreditoRepository.ExisteFolio(
                folio: entity.Folio,
                empresaId: entity.Empresa,
                excludeId: excludeId))
            erroresEncontrados.Add($"El folio de la nota de crédito: {entity.Folio}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Folio))
            erroresEncontrados.Add("El folio de la nota de crédito es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.RutCliente))
            erroresEncontrados.Add("El rut del cliente de la nota de crédito es obligatorio.");

        return erroresEncontrados;
    }
}