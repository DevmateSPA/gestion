using Gestion.core.model;

namespace Gestion.core.interfaces.repository;

public interface IFacturaRepository : IBaseRepository<Factura>
{
    Task<List<Factura>> FindAllByRutBetweenFecha(long empresaId, string rutCliente, DateTime fechaDesde, DateTime fechaHasta);
    Task<bool> ExisteFolio(string folio, long empresaId, long? excludeId = null);
    Task<List<string>> GetFolioList(string busquedaFolio, long empresaId);
}