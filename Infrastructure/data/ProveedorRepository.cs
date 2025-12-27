using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class ProveedorRepository : BaseRepository<Proveedor>, IProveedorRepository
{
    public ProveedorRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "proveedor", "vw_proveedor") {}

    public async Task<bool> ExisteRut(
        string rut,
        long empresaId,
        long? excludeId = null) => await ExistsByColumns(
            new Dictionary<string, object>
            {
                ["rut"] = rut,
                ["empresa"] = empresaId
            },
            excludeId);
}