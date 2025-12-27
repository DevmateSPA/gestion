using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class DocumentoNuloRepository : BaseRepository<DocumentoNulo>, IDocumentoNuloRepository
{
    public DocumentoNuloRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "documentonulo", "vw_documentonulo") {}

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