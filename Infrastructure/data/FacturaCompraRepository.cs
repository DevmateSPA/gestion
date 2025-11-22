using System.Collections.ObjectModel;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.core.model.detalles;

namespace Gestion.Infrastructure.data;

public class FacturaCompraRepository : BaseRepository<FacturaCompra>, IFacturaCompraRepository
{
    public FacturaCompraRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "facturacompra") {}

public override async Task<List<FacturaCompra>> FindAll()
{
    using var conn = await _connectionFactory.CreateConnection();
    using var cmd = (DbCommand)conn.CreateCommand();

    cmd.CommandText = $@"
        SELECT 
            f.*,
            d.id AS detalle_id,
            d.folio as detalle_folio,
            d.tipo as detalle_tipo,
            p.descripcion,
            d.entrada,
            d.salida,
            d.maquina,
            d.operario
        FROM {_tableName} f
        LEFT JOIN FACTURACOMPRAPRODUCTO d
        ON f.folio = d.folio
        LEFT JOIN PRODUCTO p
        ON p.codigo = d.producto";

    using var reader = await cmd.ExecuteReaderAsync();

    var facturas = new Dictionary<long, FacturaCompra>();

    while (await reader.ReadAsync())
    {
        long facturaId = reader.GetInt64(reader.GetOrdinal("id"));

        if (!facturas.TryGetValue(facturaId, out var factura))
        {
            factura = MapEntity(reader);
            factura.Detalles = new ObservableCollection<Detalle>();
            facturas.Add(facturaId, factura);
        }

        if (reader["detalle_id"] != DBNull.Value)
        {
            var detalle = new Detalle
            {
                Id = reader.GetInt64(reader.GetOrdinal("detalle_id")),
                Folio = reader.GetString(reader.GetOrdinal("detalle_folio")),
                Tipo = reader.GetString(reader.GetOrdinal("detalle_tipo")),
                Producto = reader.GetString(reader.GetOrdinal("descripcion")),
                Entrada = reader.GetInt64(reader.GetOrdinal("entrada")),
                Salida = reader.GetInt64(reader.GetOrdinal("salida")),
                Maquina = reader.GetString(reader.GetOrdinal("maquina")),
                Operario = reader.GetString(reader.GetOrdinal("operario"))
            };

            factura.Detalles.Add(detalle);
        }
    }

    return facturas.Values.ToList();
}
}