using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.Infrastructure.Services;

namespace Gestion.Infrastructure.data;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(MySqlConnectionFactory connectionFactory)
        : base(connectionFactory, "cliente") {}

    public override Task<Cliente> Save(Cliente entity)
    {
        throw new NotImplementedException();
    }
}