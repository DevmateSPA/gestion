using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class OperarioRepository : BaseRepository<Operario>, IOperarioRepository
{
    public OperarioRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "operario") {}

    public override Task<List<Operario>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_operario", "empresa = @empresa", null, null, p);
    }
}