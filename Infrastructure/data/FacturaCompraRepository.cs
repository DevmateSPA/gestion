using System.Collections.ObjectModel;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class FacturaCompraRepository : BaseRepository<FacturaCompra>, IFacturaCompraRepository
{
    public FacturaCompraRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "facturacompra") {}

    public async Task<List<FacturaCompra>> FindAllWithDetails()
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
        cmd.CommandText = $@"
            SELECT 
                f.*,
                d.id as id_detalle,
                d.producto,
                d.precio,
                d.cantidad
            FROM {_tableName} f
            LEFT JOIN FACTURADETALLE d
            ON f.folio = d.folio";

        using var reader = await cmd.ExecuteReaderAsync();

        var facturas = new List<FacturaCompra>();
        FacturaCompra? facturaActual = null;
        string? folioActual = null;

        while (await reader.ReadAsync())
        {
            string folio = reader.GetString(reader.GetOrdinal("folio"));

            if (folioActual != folio)
            {
                facturaActual = new FacturaCompra
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    RutCliente = reader.GetString(reader.GetOrdinal("rutcliente")),
                    Folio = folio,
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    Tipo = reader.GetString(reader.GetOrdinal("tipo")),
                    Detalles = new ObservableCollection<Detalle>()
                };

                facturas.Add(facturaActual);
                folioActual = folio;
            }

            int idxDetalle = reader.GetOrdinal("id_detalle");
            if (reader.IsDBNull(idxDetalle))
                continue;

            var detalle = new Detalle
            {
                Id = reader.GetInt32(idxDetalle),
                Folio = folio,
                Producto = reader.IsDBNull(reader.GetOrdinal("producto"))
                    ? ""
                    : reader.GetString(reader.GetOrdinal("producto")),
                Precio = reader.IsDBNull(reader.GetOrdinal("precio"))
                    ? 0
                    : reader.GetInt32(reader.GetOrdinal("precio")),
                Cantidad = reader.IsDBNull(reader.GetOrdinal("cantidad"))
                    ? 0
                    : reader.GetInt32(reader.GetOrdinal("cantidad"))
            };

            facturaActual.Detalles.Add(detalle);
        }

        return facturas;
    }
}