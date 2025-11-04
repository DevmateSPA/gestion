using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class FotomecanicaRepository : BaseRepository<Fotomecanica>, IFotomecanicaRepository
{
    public FotomecanicaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "fotomecanica") {}

    public override Task<Fotomecanica> Save(Fotomecanica entity)
    {
        throw new NotImplementedException();
    }
}