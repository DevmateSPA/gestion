using System.Collections.ObjectModel;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class FacturaRepository : BaseRepository<Factura>, IFacturaRepository
{
    public FacturaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "factura") { }

    public async Task<List<Factura>> FindAllWithDetails()
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
            cmd.CommandText = $@"
                SELECT 
                    f.id AS factura_id,
                    f.rutcliente,
                    f.folio,
                    f.fecha,
                    d.id AS detalle_id,
                    d.producto,
                    d.cantidad,
                    d.precio
                FROM {_tableName} f
                LEFT JOIN FACTURADETALLE d ON f.folio = d.folio
                ORDER BY f.folio;";

        using var reader = await cmd.ExecuteReaderAsync();

        var facturas = new List<Factura>();
        Factura? facturaActual = null;
        string? folioActual = null;

        while (await reader.ReadAsync())
        {
            string folio = reader.GetString(reader.GetOrdinal("folio"));

            if (folioActual != folio)
            {
                facturaActual = new Factura
                {
                    Id = reader.GetInt32(reader.GetOrdinal("factura_id")),
                    RutCliente = reader.GetString(reader.GetOrdinal("rutcliente")),
                    Folio = folio,
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    Detalles = new ObservableCollection<Detalle>()
                };

                facturas.Add(facturaActual);
                folioActual = folio;
            }

            if (!reader.IsDBNull(reader.GetOrdinal("detalle_id")))
            {
                var detalle = new Detalle()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("detalle_id")),
                    Folio = folio,
                    Producto = reader.GetString(reader.GetOrdinal("producto")),
                    Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                    Precio = reader.GetInt32(reader.GetOrdinal("precio"))
                };

                facturaActual!.Detalles.Add(detalle);
            }
        }

        return facturas;
    }
}