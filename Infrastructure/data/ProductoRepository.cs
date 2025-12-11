using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class ProductoRepository : BaseRepository<Producto>, IProductoRepository
{
    public ProductoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "producto") {}

    public override Task<List<Producto>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_producto", "empresa = @empresa", null, null, p);
    }
}