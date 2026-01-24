using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class ImpresionRepository : BaseRepository<Impresion>, IImpresionRepository
{
    public ImpresionRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "impresion", "vw_impresion") {}

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