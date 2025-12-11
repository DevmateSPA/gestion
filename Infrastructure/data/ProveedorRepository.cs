using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class ProveedorRepository : BaseRepository<Proveedor>, IProveedorRepository
{
    public ProveedorRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "proveedor") {}
    public override Task<List<Proveedor>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_proveedor", "empresa = @empresa", null, null, p);
    }
}