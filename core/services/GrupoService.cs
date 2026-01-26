using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class GrupoService : BaseService<Grupo>, IGrupoService
{
    private readonly IGrupoRepository _grupoRepository;
    public GrupoService(IGrupoRepository grupoRepository)
        :base(grupoRepository)
    {
        _grupoRepository = grupoRepository;
    }

    protected override IEnumerable<IReglaNegocio<Grupo>> DefinirReglas(
        Grupo entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Grupo>(
                c => c.Descripcion,
                "La descripción del grupo es obligatorio."),

            new UnicoRegla<Grupo>(
                existe: (g, id) =>
                    _grupoRepository.ExisteCodigo(
                        g.Codigo,
                        g.Empresa,
                        id),

                valor: g => g.Codigo,

                mensaje: "El código del grupo: {0}, ya existe para la empresa actual.")
        ];
    }
}