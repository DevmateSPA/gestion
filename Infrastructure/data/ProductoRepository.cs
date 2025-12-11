using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class ProductoRepository : BaseRepository<Producto>, IProductoRepository
{
    public ProductoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "producto", "vw_producto") {}
}