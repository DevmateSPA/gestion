using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IDocumentoNuloRepository : IBaseRepository<DocumentoNulo>
{
    Task<bool> ExisteFolio(string folio, long empresaId);
}