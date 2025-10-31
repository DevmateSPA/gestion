using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.Infrastructure.Services;

namespace Gestion.Infrastructure.data;

public class MaquinaRepository : BaseRepository<Maquina>, IMaquinaRepository
{
    public MaquinaRepository(MySqlConnectionFactory connectionFactory)
        : base(connectionFactory, "maquina") {}

    public override Task<Maquina> Save(Maquina entity)
    {
        throw new NotImplementedException();
    }
}