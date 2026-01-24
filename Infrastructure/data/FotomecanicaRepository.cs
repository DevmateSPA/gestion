using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class FotomecanicaRepository : BaseRepository<Fotomecanica>, IFotomecanicaRepository
{
    public FotomecanicaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "fotomecanica", "vw_fotomecanica") {}

    public async Task<bool> ExisteCodigo(
        string codigo,
        long empresaId,
        long? excludeId = null) => await ExistsByColumns(
            new Dictionary<string, object>
            {
                ["codigo"] = codigo,
                ["empresa"] = empresaId
            },
            excludeId);
}