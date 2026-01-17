using System.Collections.ObjectModel;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.core.model.detalles;
using Gestion.core.session;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class OrdenTrabajoRepository : BaseRepository<OrdenTrabajo>, IOrdenTrabajoRepository
{
    public OrdenTrabajoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "ordentrabajo", "vw_ordentrabajo") {}

    public async Task<long> ContarPendientes(long empresaId)
    {
        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId)
        ];

        return await CountWhere(
            where: "empresa = @empresa AND ordenentregada IS NULL",
            parameters: parameters);
    }

    public async Task<long> ContarByMaquinaWhereEmpresaAndPendientes(long empresaId, string codigoMaquina)
    {
        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId),
            new MySqlParameter("@codigoMaquina", codigoMaquina)
        ];

        return await CountWhere(
            where: "empresa = @empresa AND maquina1 = @codigoMaquina AND ordenentregada IS NULL",
            parameters: parameters);
    }

    public async Task<List<OrdenTrabajo>> FindAllByEmpresaAndPendiente(long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId)
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

        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId),
            new MySqlParameter("@codigoMaquina", codigoMaquina)
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
        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId),
            new MySqlParameter("@codigoMaquina", codigoMaquina)
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
        DbParameter[] parameters =
        [
            new MySqlParameter("@empresa", empresaId)
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