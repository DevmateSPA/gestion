using System.Data;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class IngresoClienteRepository : BaseRepository<IngresoCliente>, IIngresoClienteRepository
{
    public IngresoClienteRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "vw_ingreso_cliente", "vw_ingreso_cliente") {}

    public async Task<List<IngresoCliente>> FindAllByEmpresaAndFecha(long empresaId, DateTime desde, DateTime hasta)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = "sp_ingreso_cliente";
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add(CreateParam(cmd, "p_empresa", empresaId));
        cmd.Parameters.Add(CreateParam(cmd, "p_desde", desde));
        cmd.Parameters.Add(CreateParam(cmd, "p_hasta", hasta));
        cmd.Parameters.Add(CreateParam(cmd, "p_limit", DBNull.Value));
        cmd.Parameters.Add(CreateParam(cmd, "p_offset", DBNull.Value));

        using var reader = await cmd.ExecuteReaderAsync();

        List<IngresoCliente> list = [];

        while (await reader.ReadAsync())
            list.Add(MapEntity(reader));

        return list;
    }


    public async Task<List<IngresoCliente>> FindPageByEmpresaAndFecha(long empresaId, DateTime desde, DateTime hasta, int pageNumber, int pageSize)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = "sp_ingreso_cliente";
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add(CreateParam(cmd, "p_empresa", empresaId));
        cmd.Parameters.Add(CreateParam(cmd, "p_desde", desde));
        cmd.Parameters.Add(CreateParam(cmd, "p_hasta", hasta));
        cmd.Parameters.Add(CreateParam(cmd, "p_limit", pageSize));
        cmd.Parameters.Add(CreateParam(cmd, "p_offset", (pageNumber - 1) * pageSize));

        using var reader = await cmd.ExecuteReaderAsync();

        List<IngresoCliente> list = [];

        while (await reader.ReadAsync())
            list.Add(MapEntity(reader));

        return list;
    }
}