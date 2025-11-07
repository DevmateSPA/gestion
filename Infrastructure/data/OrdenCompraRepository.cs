using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class OrdenCompraRepository : BaseRepository<OrdenCompra>, IOrdenCompraRepository
{
    public OrdenCompraRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "ordencompra") {}
}