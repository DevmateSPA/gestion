using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IGuiaDespachoRepository : IBaseRepository<GuiaDespacho>
{
    Task<List<string>> GetFolioList(string busquedaFolio, long empresaId);
    Task<string> GetSiguienteFolio(long empresaId);
}