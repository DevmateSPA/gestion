using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.Infrastructure.Services;

namespace Gestion.Infrastructure.data;

public class BancoRepository : BaseRepository<Banco>, IBancoRepository
{
    public BancoRepository(MySqlConnectionFactory connectionFactory)
        : base(connectionFactory, "banco") {}

    public override Task<Banco> Save(Banco entity)
    {
        throw new NotImplementedException();
    }
}