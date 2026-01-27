using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class OperarioRepository : BaseRepository<Operario>, IOperarioRepository
{
    public OperarioRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "operario", "vw_operario") {}
}