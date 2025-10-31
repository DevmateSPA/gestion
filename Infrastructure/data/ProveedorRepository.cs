using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class ProveedorRepository : BaseRepository<Proveedor>, IProveedorRepository
{
    public ProveedorRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "proveedor") {}

    public override Task<Proveedor> Save(Proveedor entity)
    {
        throw new NotImplementedException();
    }
}