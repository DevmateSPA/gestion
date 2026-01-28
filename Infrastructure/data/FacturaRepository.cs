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

    public async Task<List<Factura>> FindAllByRutBetweenFecha(
        long empresaId,
        string rutCliente,
        DateTime fechaDesde,
        DateTime fechaHasta)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        return await CreateQueryBuilder()
            .WhereEqual("empresa", empresaId)
            .WhereEqual("rutcliente", rutCliente)
            .WhereBetween("fecha", fechaDesde, fechaHasta)
            .OrderBy("fecha DESC")
            .ToListAsync<Factura>();
    }

    public async Task<List<string>> GetFolioList(string numero, long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        if (string.IsNullOrWhiteSpace(numero))
            return [];

        return await CreateQueryBuilder()
            .Select("folio")
            .WhereEqual("empresa", empresaId)
            .WhereLike("folio", $"%{numero}%")
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