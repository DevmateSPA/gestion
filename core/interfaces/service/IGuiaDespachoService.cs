using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IGuiaDespachoService : IBaseService<GuiaDespacho>
{
    Task<List<string>> GetFolioList(string busquedaFolio, long empresaId);
    Task<string> GetSiguienteFolio(long empresaId);
}