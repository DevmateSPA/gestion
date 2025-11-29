using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class ImpresionRepository : BaseRepository<Impresion>, IImpresionRepository
{
    public ImpresionRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "impresion") {}

    public override Task<List<Impresion>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_impresion", "empresa = @empresa", p);
    }
}