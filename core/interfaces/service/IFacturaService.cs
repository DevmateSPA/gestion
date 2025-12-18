using Gestion.core.model;

namespace Gestion.core.interfaces.service;
public interface IFacturaService : IBaseService<Factura>
{
    Task<List<Factura>> FindAllByRutBetweenFecha(long empresaId, string rutCliente, DateTime fechaDesde, DateTime fechaHasta);
}