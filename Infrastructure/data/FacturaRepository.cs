using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class FacturaRepository : BaseRepository<Factura>, IFacturaRepository
{
    public FacturaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "vw_factura") { }

    public Task<List<Factura>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);
        return base.FindWhere("empresa = @empresa", p);
    }
}