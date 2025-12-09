using Gestion.core.model;
using Gestion.core.model.detalles;

namespace Gestion.core.interfaces.service;
public interface IDetalleOTService : IBaseService<DetalleOrdenTrabajo>
{
    Task<List<DetalleOrdenTrabajo>> FindByFolio(string folio);
    Task<bool> SaveAll(List<DetalleOrdenTrabajo> detalles);
    Task<bool> UpdateAll(IList<DetalleOrdenTrabajo> detalles);
    Task<bool> DeleteByIds(IList<long> ids);
    Task<bool> DeleteByFolio(string folio);
}