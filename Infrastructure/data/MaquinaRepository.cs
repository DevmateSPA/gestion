using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class MaquinaRepository : BaseRepository<Maquina>, IMaquinaRepository
{
    public MaquinaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "maquina", "vw_maquina") {}

    public async Task<List<Maquina>> FindMaquinaWithPendingOrders(long empresaId)
    {
        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId)
        ];

        return await FindWhereFrom(
            tableOrView: "vw_maquinas_with_pending_orders",
            where: "empresa = @empresa",
            orderBy: null,
            limit: null,
            offset: null,
            parameters: parameters);
    }
}