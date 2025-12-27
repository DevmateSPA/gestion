using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class OperarioRepository : BaseRepository<Operario>, IOperarioRepository
{
    public OperarioRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "operario", "vw_operario") {}

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