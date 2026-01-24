using System.Data;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class FacturaRepository : BaseRepository<Factura>, IFacturaRepository
{
    public FacturaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "factura", "vw_factura") { }

    public async Task<List<Factura>> FindAllByRutBetweenFecha(long empresaId, string rutCliente, DateTime fechaDesde, DateTime fechaHasta)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        DbParam[] parameters =
        [
            new DbParam("@empresa", empresaId),
            new DbParam("@rutCliente", rutCliente),
            new DbParam("@fechaDesde", fechaDesde),
            new DbParam("@fechaHasta", fechaHasta),
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

        return await CreateQueryBuilder()
            .Select("folio")
            .Where("empresa = @empresa AND rut LIKE @busquedaParam",
                new DbParam("@empresa", empresaId),
                new DbParam("@numero", $"{numero}%"))
            .ToListAsync<string>();
    }

    public async Task<string> GetSiguienteFolio(long empresaId)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = "get_siguiente_folio_fa";
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add(CreateParam(cmd, "p_empresa_id", empresaId));

        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            throw new InvalidOperationException("No se pudo generar el folio.");

        return reader.GetString("ultimo_folio");
    }
}