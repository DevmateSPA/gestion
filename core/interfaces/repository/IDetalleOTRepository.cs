using Gestion.core.model;
using Gestion.core.model.detalles;

namespace Gestion.core.interfaces.repository;

public interface IDetalleOTRepository : IBaseRepository<DetalleOrdenTrabajo>
{
    Task<List<DetalleOrdenTrabajo>> FindByFolio(string folio);
    Task<bool> SaveAll(IList<DetalleOrdenTrabajo> detalles);
    Task<bool> UpdateAll(IList<DetalleOrdenTrabajo> detalles);
    Task<bool> DeleteByIds(IList<long> ids);
    Task<bool> DeleteByFolio(string folio);
}