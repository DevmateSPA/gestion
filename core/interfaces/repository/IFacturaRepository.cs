using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IFacturaRepository : IBaseRepository<Factura>
{
    Task<List<Factura>> FindAllByRutBetweenFecha(long empresaId, string rutCliente, DateTime fechaDesde, DateTime fechaHasta);
}