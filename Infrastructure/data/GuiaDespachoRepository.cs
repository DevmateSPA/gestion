using System.Data;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class GuiaDespachoRepository : BaseRepository<GuiaDespacho>, IGuiaDespachoRepository
{
    public GuiaDespachoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "guiadespacho", "vw_guiadespacho") {}

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

    public async Task<List<string>> GetFolioList(string busquedaFolio, long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        if (string.IsNullOrWhiteSpace(busquedaFolio))
            return [];

        DbParameter[] parameters =
        [
            new MySqlParameter("@busquedaFolio", $"%{busquedaFolio}%"),
            new MySqlParameter("@empresa", empresaId),
        ];

        return await GetColumnList<string>(
            columnName: "folio",
            where: "empresa = @empresa AND folio LIKE @busquedaFolio",
            parameters: parameters);
    }

    public async Task<string> GetSiguienteFolio(long empresaId)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = "get_siguiente_folio_gd";
        cmd.CommandType = CommandType.StoredProcedure;

        var param = cmd.CreateParameter();
        param.ParameterName = "p_empresa_id";
        param.Value = empresaId;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            throw new InvalidOperationException("No se pudo generar el folio.");

        return reader.GetString("ultimo_folio");
    }
}