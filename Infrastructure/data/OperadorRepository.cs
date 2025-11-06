using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class OperadorRepository : BaseRepository<Operador>, IOperadorRepository
{
    public OperadorRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "operador") {}
}