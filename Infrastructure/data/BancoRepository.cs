using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class BancoRepository : BaseRepository<Banco>, IBancoRepository
{
    public BancoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "banco") {}

    public override Task<Banco> Save(Banco entity)
    {
        throw new NotImplementedException();
    }
}