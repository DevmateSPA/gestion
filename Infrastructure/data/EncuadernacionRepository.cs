using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class EncuadernacionRepository : BaseRepository<Encuadernacion>, IEncuadernacionRepository
{
    public EncuadernacionRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "encuadernacion") {}

    public override Task<List<Encuadernacion>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_encuadernacion", "empresa = @empresa", p);
    }
}