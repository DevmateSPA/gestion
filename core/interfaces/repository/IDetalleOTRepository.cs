using Gestion.core.model;
using Gestion.core.model.detalles;

namespace Gestion.core.interfaces.repository;

public interface IDetalleOTRepository : IBaseRepository<DetalleOrdenTrabajo>
{
    Task<List<DetalleOrdenTrabajo>> FindByFolio(string folio);
}