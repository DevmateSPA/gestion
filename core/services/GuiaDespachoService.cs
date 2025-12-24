using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class GuiaDespachoService : BaseService<GuiaDespacho>, IGuiaDespachoService
{
    private readonly IGuiaDespachoRepository _guiaDespachoRepository;
    public GuiaDespachoService(IGuiaDespachoRepository guiaDespachoRepository)
        :base(guiaDespachoRepository)
    {
        _guiaDespachoRepository = guiaDespachoRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(GuiaDespacho entity)
    {
        List<string> erroresEncontrados = [];

        if (await _guiaDespachoRepository.ExisteFolio(folio: entity.Folio, empresaId: entity.Empresa))
            erroresEncontrados.Add($"El folio de la guia de despacho: {entity.Folio}, ya existe para la empresa actual.");

        if (string.IsNullOrWhiteSpace(entity.Folio))
            erroresEncontrados.Add("El folio de la guia de despacho es obligatorio.");

        if (string.IsNullOrWhiteSpace(entity.RutCliente))
            erroresEncontrados.Add("El rut del cliente de la guia de despacho es obligatorio.");

        return erroresEncontrados;
    }
}