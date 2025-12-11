using System.Collections.ObjectModel;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.core.model.detalles;
using Gestion.core.session;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class FacturaCompraRepository : BaseRepository<FacturaCompra>, IFacturaCompraRepository
{
    public FacturaCompraRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "facturacompra", "vw_facturacompra") {}
}