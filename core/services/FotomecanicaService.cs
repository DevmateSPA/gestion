using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class FotomecanicaService : BaseService<Fotomecanica>, IFotomecanicaService
{
    private readonly IFotomecanicaRepository _fotomecanicaRepository;
    public FotomecanicaService(IFotomecanicaRepository fotomecanicaRepository)
        :base(fotomecanicaRepository)
    {
        _fotomecanicaRepository = fotomecanicaRepository;
    }

    protected override IEnumerable<IReglaNegocio<Fotomecanica>> DefinirReglas(
        Fotomecanica entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Fotomecanica>(
                f => f.Descripcion,
                "La descripción de la fotomecánica es obligatoria."),

            new UnicoRegla<Fotomecanica>(
                existe: (f, id) =>
                    _fotomecanicaRepository.ExisteCodigo(
                        f.Codigo,
                        f.Empresa,
                        id),

                valor: f => f.Codigo,

                mensaje: "El código de la fotomecánica: {0}, ya existe para la empresa actual.")
        ];
    }
}