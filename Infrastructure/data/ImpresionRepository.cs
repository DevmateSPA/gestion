using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class ImpresionRepository : BaseRepository<Impresion>, IImpresionRepository
{
    public ImpresionRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "impresion", "vw_impresion") {}
}