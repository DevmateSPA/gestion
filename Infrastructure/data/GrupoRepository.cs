using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class GrupoRepository : BaseRepository<Grupo>, IGrupoRepository
{
    public GrupoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "grupo") {}

    public override Task<List<Grupo>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_grupo", "empresa = @empresa", p);
    }
}