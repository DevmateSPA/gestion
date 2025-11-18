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
                    f.*,
                    d.id as id_detalle,
                    d.producto,
                    d.precio,
                    d.cantidad
                FROM {_tableName} f
                LEFT JOIN FACTURADETALLE d
                ON f.folio = d.folio";

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
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    RutCliente = reader.GetString(reader.GetOrdinal("rutcliente")),
                    Folio = folio,
                    Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                    FechaVencimiento = reader.GetDateTime(reader.GetOrdinal("fechavencimiento")),
                    OrdenTrabajo = reader.GetString(reader.GetOrdinal("ordentrabajo")),
                    NotaCredito = reader.GetInt32(reader.GetOrdinal("notacredito")),
                    TipoCredito = reader.GetInt32(reader.GetOrdinal("tipocredito")),
                    Detalles = new ObservableCollection<Detalle>()
                };

                facturas.Add(facturaActual);
                folioActual = folio;
            }

            if (!reader.IsDBNull(reader.GetOrdinal("id_detalle")) && facturaActual != null)
            {
                var detalle = new Detalle
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id_detalle")),
                    Folio = folio,
                    Producto = reader.GetString(reader.GetOrdinal("producto")),
                    Precio = reader.GetInt32(reader.GetOrdinal("precio")),
                    Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad"))
                };

                facturaActual.Detalles.Add(detalle);
            }
        }

        return facturas;
    }
}