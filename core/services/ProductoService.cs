using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;
using Gestion.core.reglas.common;

namespace Gestion.core.services;

public class ProductoService : BaseService<Producto>, IProductoService
{
    private readonly IProductoRepository _productoRepository;
    public ProductoService(IProductoRepository productoRepository)
        :base(productoRepository)
    {
        _productoRepository = productoRepository;
    }

    protected override IEnumerable<IReglaNegocio<Producto>> DefinirReglas(
        Producto entity,
        long? excludeId = null)
    {
        return
        [
            new RequeridoRegla<Producto>(
                c => c.Descripcion,
                "La descripción del producto es obligatoria."),

            new UnicoRegla<Producto>(
                existe: (p, id) =>
                    _productoRepository.ExistsByColumns(
                        [
                            ("codigo", p.Codigo),
                            ("empresa", p.Empresa)
                        ],
                        id),

                valor: p => p.Codigo,

                mensaje: "El código del producto: {0}, ya existe para la empresa actual.")
        ];
    }
}