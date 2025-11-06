using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class MaquinaRepository : BaseRepository<Maquina>, IMaquinaRepository
{
    public MaquinaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "maquina") {}
}