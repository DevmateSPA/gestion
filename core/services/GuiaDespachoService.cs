using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class GuiaDespachoService : BaseService<GuiaDespacho>, IGuiaDespachoService
{
    private readonly IGuiaDespachoRepository _guiaDespachoRepository;
    public GuiaDespachoService(IGuiaDespachoRepository guiaDespachoRepository)
        :base(guiaDespachoRepository)
    {
        _guiaDespachoRepository = guiaDespachoRepository;
    }

    public async Task<List<string>> GetFolioList(string busquedaFolio, long empresaId)
    {
        return await _guiaDespachoRepository.GetFolioList(busquedaFolio, empresaId);
    }

    protected override IEnumerable<IReglaNegocio<GuiaDespacho>> DefinirReglas(
        GuiaDespacho entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<GuiaDespacho>(
                gd => gd.Folio,
                "El folio de la guia de despacho es obligatorio."),

            new RequeridoRegla<GuiaDespacho>(
                gd => gd.RutCliente,
                "El rut del cliente de la guia de despacho es obligatorio."),

            new UnicoRegla<GuiaDespacho>(
                existe: (gd, id) =>
                    _guiaDespachoRepository.ExisteFolio(
                        gd.Folio,
                        gd.Empresa,
                        id),

                valor: gd => gd.Folio,

                mensaje: "El folio de la guia de despacho: {0}, ya existe para la empresa actual.")
        ];
    }

    public async Task<string> GetSiguienteFolio(long empresaId)
    {
        return await _guiaDespachoRepository.GetSiguienteFolio(empresaId);
    }
}