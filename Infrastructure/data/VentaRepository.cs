using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class VentaRepository : BaseRepository<Venta>, IVentaRepository
{
    public VentaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "venta", "vw_venta") {}
}