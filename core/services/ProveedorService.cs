using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class ProveedorService : BaseService<Proveedor>, IProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;
    public ProveedorService(IProveedorRepository proveedorRepository)
        :base(proveedorRepository)
    {
        _proveedorRepository = proveedorRepository;
    }

    protected override IEnumerable<IReglaNegocio<Proveedor>> DefinirReglas(
        Proveedor entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Proveedor>(
                c => c.Rut,
                "El rut del proveedor es obligatorio."),

            new UnicoRegla<Proveedor>(
                existe: (p, id) =>
                    _proveedorRepository.ExistsByColumns(
                        [
                            ("rut", p.Rut),
                            ("empresa", p.Empresa)
                        ],
                        id),

                valor: p => p.Rut,

                mensaje: "El rut del proveedor: {0}, ya existe para la empresa actual.")
        ];
    }
}