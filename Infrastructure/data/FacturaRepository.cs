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

    public async Task<bool> ExisteFolio(
        string folio,
        long empresaId,
        long? excludeId = null) => await ExistsByColumns(
            new Dictionary<string, object>
            {
                ["folio"] = folio,
                ["empresa"] = empresaId
            },
            excludeId);

    public async Task<List<string>> GetFolioList(string numero, long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        if (string.IsNullOrWhiteSpace(numero))
            return [];

        DbParameter[] parameters =
        [
            new MySqlParameter("@numero", $"%{numero}%"),
            new MySqlParameter("@empresa", empresaId),
        ];

        return await GetColumnList<string>(
            columnName: "folio",
            where: "empresa = @empresa AND folio LIKE @numero",
            parameters: parameters);
    }

    public async Task<String> GetSiguienteFolio(long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId),
        ];

        return await GetColumnList<string>(
            columnName: "MAX(folio)",
            where: "empresa = @empresa",
            parameters: parameters,
            orderby: "ORDER BY 1 DESC",
            limit: "LIMIT 1");
    }
}