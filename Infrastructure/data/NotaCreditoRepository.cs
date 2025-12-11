using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class NotaCreditoRepository : BaseRepository<NotaCredito>, INotaCreditoRepository
{
    public NotaCreditoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "notacredito") {}

    public override Task<List<NotaCredito>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_notacredito", "empresa = @empresa", null, null, p);
    }
}