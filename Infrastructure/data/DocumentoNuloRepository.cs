using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class DocumentoNuloRepository : BaseRepository<DocumentoNulo>, IDocumentoNuloRepository
{
    public DocumentoNuloRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "documentonulo", "vw_documentonulo") {}
}