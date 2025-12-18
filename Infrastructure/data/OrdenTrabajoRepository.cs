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
}