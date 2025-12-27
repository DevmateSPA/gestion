using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "cliente", "vw_cliente") {}

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