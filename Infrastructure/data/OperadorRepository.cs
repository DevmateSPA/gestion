using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.Infrastructure.Services;

namespace Gestion.Infrastructure.data;

public class OperadorRepository : BaseRepository<Operador>, IOperadorRepository
{
    public OperadorRepository(MySqlConnectionFactory connectionFactory)
        : base(connectionFactory, "operador") {}

    public override Task<Operador> Save(Operador entity)
    {
        throw new NotImplementedException();
    }
}