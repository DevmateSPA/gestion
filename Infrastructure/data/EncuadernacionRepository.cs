using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class EncuadernacionRepository : BaseRepository<Encuadernacion>, IEncuadernacionRepository
{
    public EncuadernacionRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "encuadernacion", "vw_encuadernacion") {}
}