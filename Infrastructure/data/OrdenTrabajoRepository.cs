using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class OrdenTrabajoRepository : BaseRepository<OrdenTrabajo>, IOrdenTrabajoRepository
{
    public OrdenTrabajoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "ordentrabajo") {}

    public override Task<List<OrdenTrabajo>> FindAllByEmpresa(long empresaId)
    {
         var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_ordentrabajo", "empresa = @empresa", p);
    }
}