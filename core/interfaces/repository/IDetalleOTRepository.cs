using Gestion.core.model;
using Gestion.core.model.detalles;

namespace Gestion.core.interfaces.repository;

public interface IDetalleOTRepository : IBaseRepository<DetalleOrdenTrabajo>
{
    Task<List<DetalleOrdenTrabajo>> FindByFolio(string folio, long empresa);
    Task<bool> SaveAll(IList<DetalleOrdenTrabajo> detalles);
    Task<bool> UpdateAll(IList<DetalleOrdenTrabajo> detalles, long empresaId);
    Task<bool> DeleteByIds(IList<long> ids, long empresaId);
    Task<bool> DeleteByFolio(string folio, long empresaId);
}