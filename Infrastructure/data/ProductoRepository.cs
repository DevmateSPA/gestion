using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class ProductoRepository : BaseRepository<Producto>, IProductoRepository
{
    public ProductoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "producto") {}

    public override Task<Producto> Save(Producto entity)
    {
        throw new NotImplementedException();
    }
}