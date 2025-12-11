using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class DocumentoNuloRepository : BaseRepository<DocumentoNulo>, IDocumentoNuloRepository
{
    public DocumentoNuloRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "documentonulo") {}

    public override Task<List<DocumentoNulo>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_documentonulo", "empresa = @empresa", null, null, p);
    }
}