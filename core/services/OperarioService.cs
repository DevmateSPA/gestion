using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class OperarioService : BaseService<Operario>, IOperarioService
{
    private readonly IOperarioRepository _operarioRepository;
    public OperarioService(IOperarioRepository operarioRepository)
        :base(operarioRepository)
    {
        _operarioRepository = operarioRepository;
    }

    protected override IEnumerable<IReglaNegocio<Operario>> DefinirReglas(
        Operario entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Operario>(
                o => o.Nombre,
                "El nombre del operario es obligatorio."),

            new UnicoRegla<Operario>(
                existe: (op, id) =>
                    _operarioRepository.ExistsByColumns(
                        [
                            ("codigo", op.Codigo),
                            ("empresa", op.Empresa)
                        ],
                        id),

                valor: op => op.Codigo,

                mensaje: "El código del operario: {0}, ya existe para la empresa actual.")
        ];
    }
}