using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class FacturaCompraProductoRepository : BaseRepository<FacturaCompraProducto>, IFacturaCompraProductoRepository
{
    public FacturaCompraProductoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "facturacompraproducto") {}

    public async Task<List<FacturaCompraProducto>> FindByFolio(string folio)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
        cmd.CommandText = $"""
        SELECT 
            d.id,
            d.tipo,
            d.folio,
            d.producto,
            d.entrada,
            d.salida,
            d.maquina,
            d.operario,
            d.fecha
        FROM {_tableName} d
        WHERE folio = @folio;
        """;

        var param = cmd.CreateParameter();
        param.ParameterName = "@folio";
        param.Value = folio;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        var list = new List<FacturaCompraProducto>();
        while (await reader.ReadAsync())
            list.Add(MapEntity(reader));

        return list;
    }

    public override Task<List<FacturaCompraProducto>> FindAllByEmpresa(long empresaId)
    {
        var p = new MySqlParameter("@empresa", empresaId);

        return FindWhereFrom("vw_facturacompraproducto", "empresa = @empresa", p);
    }
}