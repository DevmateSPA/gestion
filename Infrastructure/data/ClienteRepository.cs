using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "cliente") {}

    public override Task<Cliente> Save(Cliente entity)
    {
        throw new NotImplementedException();
    }
}