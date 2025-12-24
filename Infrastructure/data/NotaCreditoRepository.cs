using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class NotaCreditoRepository : BaseRepository<NotaCredito>, INotaCreditoRepository
{
    public NotaCreditoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "notacredito", "vw_maquina") {}

    public async Task<bool> ExisteFolio(string folio, long empresaId)
    => await ExistsByColumns(new Dictionary<string, object>
    {
        ["folio"] = folio,
        ["empresa"] = empresaId
    });
}