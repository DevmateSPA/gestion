using Gestion.core.model;
using Gestion.core.model.detalles;

namespace Gestion.core.interfaces.service;
public interface IDetalleOTService : IBaseService<DetalleOrdenTrabajo>
{
    Task<List<DetalleOrdenTrabajo>> FindByFolio(string folio, long empresaId);
    Task<bool> SaveAll(List<DetalleOrdenTrabajo> detalles);
    Task<bool> UpdateAll(IList<DetalleOrdenTrabajo> detalles, long empresaId);
    Task<bool> DeleteByIds(IList<long> ids, long empresaId);
    Task<bool> DeleteByFolio(string folio, long empresaId);
}