using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.Infrastructure.Services;

namespace Gestion.Infrastructure.data;

public class ProductoRepository : BaseRepository<Producto>, IProductoRepository
{
    public ProductoRepository(MySqlConnectionFactory connectionFactory)
        : base(connectionFactory, "producto") {}

    public override Task<Producto> Save(Producto entity)
    {
        throw new NotImplementedException();
    }
}