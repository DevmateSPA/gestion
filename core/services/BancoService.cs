using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class BancoService : BaseService<Banco>, IBancoService
{
    private readonly IBancoRepository _bancoRepository;
    public BancoService(IBancoRepository bancoRepository)
        :base(bancoRepository)
    {
        _bancoRepository = bancoRepository;
    }

    protected override IEnumerable<IReglaNegocio<Banco>> DefinirReglas(
        Banco entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Banco>(
                b => b.Nombre,
                "El nombre del banco es obligatorio."),

            new UnicoRegla<Banco>(
                existe: (b, id) =>
                    _bancoRepository.ExistsByColumns(
                        [
                            ("codigo", b.Codigo),
                            ("empresa", b.Empresa)
                        ],
                        id),

                valor: b => b.Codigo,

                mensaje: "El código del banco: {0}, ya existe para la empresa actual.")
        ];
    }

}