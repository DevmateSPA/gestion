using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class FotomecanicaRepository : BaseRepository<Fotomecanica>, IFotomecanicaRepository
{
    public FotomecanicaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "fotomecanica") {}

    public override Task<List<Fotomecanica>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_fotomecanica", "empresa = @empresa", null, null, p);
    }
}