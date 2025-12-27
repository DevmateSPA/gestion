using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class GuiaDespachoRepository : BaseRepository<GuiaDespacho>, IGuiaDespachoRepository
{
    public GuiaDespachoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "guiadespacho", "vw_guiadespacho") {}

    public async Task<bool> ExisteFolio(
        string folio,
        long empresaId,
        long? excludeId = null) => await ExistsByColumns(
            new Dictionary<string, object>
            {
                ["folio"] = folio,
                ["empresa"] = empresaId
            },
            excludeId);
}