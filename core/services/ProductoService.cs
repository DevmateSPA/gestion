using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using Gestion.core.model;

namespace Gestion.core.services;

public class ProductoService : BaseService<Producto>, IProductoService
{
    public ProductoService(IProductoRepository productoRepository)
        :base(productoRepository) { }
}