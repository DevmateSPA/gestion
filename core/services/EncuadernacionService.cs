using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class EncuadernacionService : BaseService<Encuadernacion>, IEncuadernacionService
{
    private readonly IEncuadernacionRepository _encuadernacionRepository;
    public EncuadernacionService(IEncuadernacionRepository encuadernacionRepository)
        :base(encuadernacionRepository)
    {
        _encuadernacionRepository = encuadernacionRepository;
    }

    protected override IEnumerable<IReglaNegocio<Encuadernacion>> DefinirReglas(
        Encuadernacion entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Encuadernacion>(
                e => e.Descripcion,
                "La descripción de la encuadernación es obligatoria."),

            new UnicoRegla<Encuadernacion>(
                existe: (e, id) =>
                    _encuadernacionRepository.ExistsByColumns(
                        [
                            ("codigo", e.Codigo),
                            ("empresa", e.Empresa)
                        ],
                        id),

                valor: e => e.Codigo,

                mensaje: "El código de la encuadernación: {0}, ya existe para la empresa actual.")
        ];
    }
}