using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class GuiaDespachoRepository : BaseRepository<GuiaDespacho>, IGuiaDespachoRepository
{
    public GuiaDespachoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "guiadespacho") {}
}