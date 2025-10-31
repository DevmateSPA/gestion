using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.Infrastructure.Services;

namespace Gestion.Infrastructure.data;

public class GrupoRepository : BaseRepository<Grupo>, IGrupoRepository
{
    public GrupoRepository(MySqlConnectionFactory connectionFactory)
        : base(connectionFactory, "grupo") {}

    public override Task<Grupo> Save(Grupo entity)
    {
        throw new NotImplementedException();
    }
}