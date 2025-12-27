using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class ProductoService : BaseService<Producto>, IProductoService
{
    private readonly IProductoRepository _productoRepository;
    public ProductoService(IProductoRepository productoRepository)
        :base(productoRepository)
    {
        _productoRepository = productoRepository;
    }

    protected override async Task<List<string>> ValidarReglasNegocio(
        Producto entity,
        long? excludeId = null)
    {
        List<string> errores = [];

        if (await _productoRepository.ExisteCodigo(
                codigo: entity.Codigo,
                empresaId: entity.Empresa,
                excludeId: excludeId))
        {
            errores.Add($"El código del producto: {entity.Codigo}, ya existe para la empresa actual.");
        }

        if (string.IsNullOrWhiteSpace(entity.Descripcion))
            errores.Add("La descripción del producto es obligatoria.");

        return errores;
    }
}