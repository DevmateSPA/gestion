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
                    f.id,
                    f.folio AS folio_factura,
                    f.rutcliente,
                    f.fecha,
                    f.fechavencimiento,
                    f.ordentrabajo,
                    f.notacredito,
                    f.tipocredito,
                    f.iva,
                    f.neto,
                    f.total,
                    d.id AS id_detalle,
                    d.producto,
                    d.precio,
                    d.cantidad
                FROM FACTURA f
                LEFT JOIN FACTURADETALLE d ON f.folio = d.folio
                ORDER BY f.folio";

        using var reader = await cmd.ExecuteReaderAsync();

        var facturasDict = new Dictionary<string, Factura>();

        while (await reader.ReadAsync())
        {
            if (reader.IsDBNull(reader.GetOrdinal("folio_factura")))
                continue;

            string folio = reader.GetString(reader.GetOrdinal("folio_factura"));

            if (!facturasDict.TryGetValue(folio, out var factura))
            {
                factura = new Factura
                {
                    Id = reader.GetInt64(reader.GetOrdinal("id")),
                    RutCliente = reader.IsDBNull(reader.GetOrdinal("rutcliente"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("rutcliente")),
                    Folio = folio,
                    Fecha = reader.IsDBNull(reader.GetOrdinal("fecha"))
                                ? default
                                : reader.GetDateTime(reader.GetOrdinal("fecha")),

                    FechaVencimiento = reader.IsDBNull(reader.GetOrdinal("fechavencimiento"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("fechavencimiento")),

                    OrdenTrabajo = reader.IsDBNull(reader.GetOrdinal("ordentrabajo"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("ordentrabajo")),

                    NotaCredito = reader.IsDBNull(reader.GetOrdinal("notacredito"))
                                ? 0
                                : reader.GetInt64(reader.GetOrdinal("notacredito")),

                    TipoCredito = reader.IsDBNull(reader.GetOrdinal("tipocredito"))
                                ? 0
                                : reader.GetInt64(reader.GetOrdinal("tipocredito")),
                    Iva = reader.IsDBNull(reader.GetOrdinal("iva"))
                            ? 0
                            : reader.GetInt64(reader.GetOrdinal("iva")),

                    Neto = reader.IsDBNull(reader.GetOrdinal("neto"))
                            ? 0
                            : reader.GetInt64(reader.GetOrdinal("neto")),

                    Total = reader.IsDBNull(reader.GetOrdinal("total"))
                            ? 0
                            : reader.GetInt64(reader.GetOrdinal("total")),
                    Detalles = new ObservableCollection<Detalle>()
                };

                facturasDict.Add(folio, factura);
            }

            if (!reader.IsDBNull(reader.GetOrdinal("id_detalle")))
            {
                factura.Detalles.Add(new Detalle
                {
                    Id = reader.GetInt64(reader.GetOrdinal("id_detalle")),
                    Folio = folio,

                    Producto = reader.IsDBNull(reader.GetOrdinal("producto"))
                            ? string.Empty
                            : reader.GetString(reader.GetOrdinal("producto")),

                    Precio = reader.IsDBNull(reader.GetOrdinal("precio"))
                            ? 0
                            : reader.GetInt64(reader.GetOrdinal("precio")),

                    Cantidad = reader.IsDBNull(reader.GetOrdinal("cantidad"))
                            ? 0
                            : reader.GetInt64(reader.GetOrdinal("cantidad")),
                });
            }
        }

        return facturasDict.Values.ToList();
    }
}