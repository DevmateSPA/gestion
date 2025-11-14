using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IDetalleFacturaService : IBaseService<Detalle>
{
    Task<List<Detalle>> FindByFolio(string folio);
}