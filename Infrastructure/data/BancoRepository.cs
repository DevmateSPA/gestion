using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class BancoRepository : BaseRepository<Banco>, IBancoRepository
{
    public BancoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "banco", "vw_banco") {}
}