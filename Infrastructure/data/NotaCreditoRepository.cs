using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class NotaCreditoRepository : BaseRepository<NotaCredito>, INotaCreditoRepository
{
    public NotaCreditoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "notacredito") {}
}