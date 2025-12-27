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

    public async Task<long> ContarMaquinasConPendientes(long empresaId)
    {
        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId)
        ];

        return await CountWhere(
            where: "empresa = @empresa",
            tableName: "vw_maquinas_with_pending_orders",
            parameters: parameters);
    }

    public async Task<List<Maquina>> FindAllMaquinaWithPendingOrders(long empresaId)
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

    public async Task<List<Maquina>> FindPageMaquinaWithPendingOrders(long empresaId, int pageNumber, int pageSize)
    {
        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId),
        ];

        int offset = (pageNumber - 1) * pageSize;

        return await FindWhereFrom(
            tableOrView: "vw_maquinas_with_pending_orders",
            where: "empresa = @empresa",
            orderBy: null,
            limit: pageSize,
            offset: offset,
            parameters: parameters);
    }

    public async Task<bool> ExisteCodigo(
        string codigo,
        long empresaId,
        long? excludeId = null) => await ExistsByColumns(
            new Dictionary<string, object>
            {
                ["codigo"] = codigo,
                ["empresa"] = empresaId
            },
            excludeId);
}