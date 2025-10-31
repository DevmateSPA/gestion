using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class GrupoRepository : BaseRepository<Grupo>, IGrupoRepository
{
    public GrupoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "grupo") {}

    public override Task<Grupo> Save(Grupo entity)
    {
        throw new NotImplementedException();
    }
}