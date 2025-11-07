using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class FacturaCompraProductoRepository : BaseRepository<FacturaCompraProducto>, IFacturaCompraProductoRepository
{
    public FacturaCompraProductoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "facturacompraproducto") {}
}