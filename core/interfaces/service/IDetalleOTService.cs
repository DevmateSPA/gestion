using Gestion.core.model;
using Gestion.core.model.detalles;

namespace Gestion.core.interfaces.service;
public interface IDetalleOTService : IBaseService<DetalleOrdenTrabajo>
{
    Task<List<DetalleOrdenTrabajo>> FindByFolio(string folio);
}