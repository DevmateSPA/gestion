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
        return await CreateQueryBuilder()
            .WhereEqual("empresa", empresaId)
            .WhereIsNull("ordenentregada")
            .CountAsync();
    }

    public async Task<long> ContarByMaquinaWhereEmpresaAndPendientes(long empresaId, string codigoMaquina)
    {
        return await CreateQueryBuilder()
            .WhereEqual("empresa", empresaId)
            .WhereEqual("maquina1", codigoMaquina)
            .WhereIsNull("ordenentregada")
            .CountAsync();
    }

    public async Task<List<OrdenTrabajo>> FindAllByEmpresaAndPendiente(long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        return await CreateQueryBuilder()
            .WhereEqual("empresa", empresaId)
            .WhereIsNull("ordenentregada")
            .OrderBy("fecha DESC")
            .ToListAsync<OrdenTrabajo>();
    }

    public async Task<List<OrdenTrabajo>> FindAllByMaquinaWhereEmpresaAndPendiente(long empresaId, string codigoMaquina)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        return await CreateQueryBuilder()
            .WhereEqual("empresa", empresaId)
            .WhereEqual("maquina1", codigoMaquina)
            .WhereIsNull("ordenentregada")
            .OrderBy("fecha DESC")
            .ToListAsync<OrdenTrabajo>();
    }

    public async Task<List<OrdenTrabajo>> FindPageByMaquinaWhereEmpresaAndPendiente(
        long empresaId,
        string codigoMaquina,
        int pageNumber,
        int pageSize)
    {
        return await CreateQueryBuilder()
            .WhereEqual("empresa", empresaId)
            .WhereEqual("maquina1", codigoMaquina)
            .WhereIsNull("ordenentregada")
            .Page(pageNumber, pageSize)
            .OrderBy("fecha DESC")
            .ToListAsync<OrdenTrabajo>();
    }

    public async Task<List<OrdenTrabajo>> FindPageByEmpresaAndPendiente(
        long empresaId,
        int pageNumber,
        int pageSize)
    {
        return await CreateQueryBuilder()
            .WhereEqual("empresa", empresaId)
            .WhereIsNull("ordenentregada")
            .Page(pageNumber, pageSize)
            .OrderBy("fecha DESC")
            .ToListAsync<OrdenTrabajo>();
    }

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

        cmd.CommandText = "get_siguiente_folio_ot";
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add(CreateParam(cmd, "p_empresa_id", empresaId));

        using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
            throw new InvalidOperationException("No se pudo generar el folio.");

        return reader.GetString("ultimo_folio");
    }
    
}