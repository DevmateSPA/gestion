using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class MaquinaRepository : BaseRepository<Maquina>, IMaquinaRepository
{
    public MaquinaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "maquina", "vw_maquina") {}
}