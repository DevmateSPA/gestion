using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class BancoRepository : BaseRepository<Banco>, IBancoRepository
{
    public BancoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "banco") {}

    public override Task<List<Banco>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_banco", "empresa = @empresa", null, null, p);
    }
}