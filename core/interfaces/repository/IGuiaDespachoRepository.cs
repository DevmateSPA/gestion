using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IGuiaDespachoRepository : IBaseRepository<GuiaDespacho>
{
    Task<bool> ExisteFolio(string folio, long empresaId, long? excludeId = null);
    Task<List<string>> GetFolioList(string busquedaFolio, long empresaId);
    Task<string> GetSiguienteFolio(long empresaId);
}