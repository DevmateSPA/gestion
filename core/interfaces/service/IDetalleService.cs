using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IDetalleService : IBaseService<Detalle>
{
    Task<List<Detalle>> FindByFolio(string folio);
}