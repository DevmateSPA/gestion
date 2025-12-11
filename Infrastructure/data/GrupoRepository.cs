using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class GrupoRepository : BaseRepository<Grupo>, IGrupoRepository
{
    public GrupoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "grupo", "vw_grupo") {}
}