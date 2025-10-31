using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.Infrastructure.Services;

namespace Gestion.Infrastructure.data;

public class ProveedorRepository : BaseRepository<Proveedor>, IProveedorRepository
{
    public ProveedorRepository(MySqlConnectionFactory connectionFactory)
        : base(connectionFactory, "proveedor") {}

    public override Task<Proveedor> Save(Proveedor entity)
    {
        throw new NotImplementedException();
    }
}