using System.Data;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class GuiaDespachoRepository : BaseRepository<GuiaDespacho>, IGuiaDespachoRepository
{
    public GuiaDespachoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "guiadespacho", "vw_guiadespacho") {}

    public async Task<List<string>> GetFolioList(string busquedaFolio, long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        if (string.IsNullOrWhiteSpace(busquedaFolio))
            return [];

        return await CreateQueryBuilder()
            .Select("folio")
            .WhereEqual("empresa", empresaId)
            .WhereLike("folio", $"%{busquedaFolio}%")
            .ToListAsync<string>();
    }

    public async Task<string> GetSiguienteFolio(long empresaId)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = "get_siguiente_folio_gd";
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add(CreateParam(cmd, "p_empresa_id", empresaId));

        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            throw new InvalidOperationException("No se pudo generar el folio.");

        return reader.GetString("ultimo_folio");
    }
}