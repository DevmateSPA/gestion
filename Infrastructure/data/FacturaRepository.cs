using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class FacturaRepository : BaseRepository<Factura>, IFacturaRepository
{
    public FacturaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "factura", "vw_factura") { }

    public async Task<List<Factura>> FindAllByRutBetweenFecha(long empresaId, string rutCliente, DateTime fechaDesde, DateTime fechaHasta)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId),
            new MySqlParameter("@rutCliente", rutCliente),
            new MySqlParameter("@fechaDesde", fechaDesde),
            new MySqlParameter("@fechaHasta", fechaHasta),
        ];

        return await FindWhereFrom(
            tableOrView: _viewName,
            where: "empresa = @empresa AND rutcliente = @rutCliente AND fecha BETWEEN @fechaDesde AND @fechaHasta",
            orderBy: "fecha DESC",
            limit: null,
            offset: null,
            parameters);
    }

    public async Task<bool> ExisteFolio(string folio, long empresaId)
    => await ExistsByColumns(new Dictionary<string, object>
    {
        ["folio"] = folio,
        ["empresa"] = empresaId
    });
}