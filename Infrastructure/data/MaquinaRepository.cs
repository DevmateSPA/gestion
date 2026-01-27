using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class MaquinaRepository : BaseRepository<Maquina>, IMaquinaRepository
{
    public MaquinaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "maquina", "vw_maquina") {}

    public async Task<long> ContarMaquinasConPendientes(long empresaId)
    {
        long total = await CreateQueryBuilder()
            .From("vw_maquinas_with_pending_orders")
            .Where("empresa = @empresa", new DbParam("@empresa", empresaId))
            .CountAsync();

        return total;
    }

    public async Task<List<Maquina>> FindAllMaquinaWithPendingOrders(long empresaId)
    {
        var result = await CreateQueryBuilder()
            .From("vw_maquinas_with_pending_orders")
            .Where("empresa = @empresa", new DbParam("@empresa", empresaId))
            .ToListAsync<Maquina>();

        return result;
    }

    public async Task<List<Maquina>> FindPageMaquinaWithPendingOrders(long empresaId, int pageNumber, int pageSize)
    {
        int offset = (pageNumber - 1) * pageSize;

        var builder = CreateQueryBuilder()
            .From("vw_maquinas_with_pending_orders")
            .Where("empresa = @empresa", new DbParam("@empresa", empresaId))
            .Limit(pageSize, offset);

        return await builder.ToListAsync<Maquina>();
    }
}