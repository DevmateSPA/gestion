using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class ImpresionService : BaseService<Impresion>, IImpresionService
{
    private readonly IImpresionRepository _impresionRepository;
    public ImpresionService(IImpresionRepository impresionRepository)
        :base(impresionRepository)
    {
        _impresionRepository = impresionRepository;
    }

    protected override IEnumerable<IReglaNegocio<Impresion>> DefinirReglas(
        Impresion entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Impresion>(
                i => i.Descripcion,
                "La descripción de la impresión es obligatoria."),

            new UnicoRegla<Impresion>(
                existe: (i, id) =>
                    _impresionRepository.ExistsByColumns(
                        [
                            ("codigo", i.Codigo),
                            ("empresa", i.Empresa)
                        ],
                        id),

                valor: i => i.Codigo,

                mensaje: "El código de la impresión: {0}, ya existe para la empresa actual.")
        ];
    }
}