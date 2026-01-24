using System.Data;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class OrdenTrabajoRepository : BaseRepository<OrdenTrabajo>, IOrdenTrabajoRepository
{
    public OrdenTrabajoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "ordentrabajo", "vw_ordentrabajo") {}

    public async Task<long> ContarPendientes(long empresaId)
    {
        DbParam[] parameters =
        [
            new DbParam("@empresa", empresaId)
        ];

        return await CountWhere(
            where: "empresa = @empresa AND ordenentregada IS NULL",
            parameters: parameters);
    }

    public async Task<long> ContarByMaquinaWhereEmpresaAndPendientes(long empresaId, string codigoMaquina)
    {
        DbParam[] parameters =
        [
            new DbParam("@empresa", empresaId),
            new DbParam("@codigoMaquina", codigoMaquina)
        ];

        return await CountWhere(
            where: "empresa = @empresa AND maquina1 = @codigoMaquina AND ordenentregada IS NULL",
            parameters: parameters);
    }

    public async Task<List<OrdenTrabajo>> FindAllByEmpresaAndPendiente(long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        DbParam[] parameters =
        [
            new DbParam("@empresa", empresaId)
        ];

        return await FindWhereFrom(
            tableOrView: _viewName,
            where: "empresa = @empresa AND ordenentregada IS NULL",
            orderBy: "fecha DESC",
            limit: null,
            offset: null,
            parameters);
    }

    public async Task<List<OrdenTrabajo>> FindAllByMaquinaWhereEmpresaAndPendiente(long empresaId, string codigoMaquina)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        DbParam[] parameters =
        [
            new DbParam("@empresa", empresaId),
            new DbParam("@codigoMaquina", codigoMaquina)
        ];

        return await FindWhereFrom(
            tableOrView: _viewName,
            where: "empresa = @empresa AND maquina1 = @codigoMaquina AND ordenentregada IS NULL",
            orderBy: "fecha DESC",
            limit: null,
            offset: null,
            parameters);
    }

    public async Task<List<OrdenTrabajo>> FindPageByMaquinaWhereEmpresaAndPendiente(
        long empresaId,
        string codigoMaquina,
        int pageNumber,
        int pageSize)
    {
        DbParam[] parameters =
        [
            new DbParam("@empresa", empresaId),
            new DbParam("@codigoMaquina", codigoMaquina)
        ];

        return await FindPageWhere(
            where: "empresa = @empresa AND maquina1 = @codigoMaquina AND ordenentregada IS NULL",
            orderBy: "fecha DESC",
            pageNumber: pageNumber,
            pageSize: pageSize,
            parameters);
    }

    public async Task<List<OrdenTrabajo>> FindPageByEmpresaAndPendiente(
        long empresaId,
        int pageNumber,
        int pageSize)
    {
        DbParam[] parameters =
        [
            new DbParam("@empresa", empresaId)
        ];

        return await FindPageWhere(
            where: "empresa = @empresa AND ordenentregada IS NULL",
            orderBy: "fecha DESC",
            pageNumber: pageNumber,
            pageSize: pageSize,
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

    public async Task<List<string>> GetFolioList(string busquedaFolio, long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        if (string.IsNullOrWhiteSpace(busquedaFolio))
            return [];

        return await CreateQueryBuilder()
            .Select("folio")
            .Where("empresa = @empresa AND folio LIKE @busquedaFolio",
                new DbParam("@empresa", empresaId),
                new DbParam("@busquedaFolio", $"{busquedaFolio}%"))
            .ToListAsync<string>(); 
    }

    public async Task<string> GetSiguienteFolio(long empresaId)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = "get_siguiente_folio_ot";
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add(CreateParam(cmd, "p_empresa_id", empresaId));

        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            throw new InvalidOperationException("No se pudo generar el folio.");

        return reader.GetString("ultimo_folio");
    }
    
}