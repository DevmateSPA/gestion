using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class FacturaRepository : BaseRepository<Factura>, IFacturaRepository
{
    public FacturaRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "factura") { }

    public async Task<List<Factura>> FindAllConDetalles()
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
        
        var facturasDict = new Dictionary<string, Factura>();

        cmd.CommandText = @"
            SELECT 
                f.id,
                f.fecha,
                f.folio,
                f.rutcliente,
                fd.id,
                fd.producto,
                fd.folio,
                fd.cantidad AS total_cantidad,
                f.total
            FROM FACTURA f
            JOIN FACTURADETALLE fd 
            ON f.folio = fd.folio;
        ";

        using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        var folio = reader.IsDBNull(reader.GetOrdinal("folio"))
            ? string.Empty
            : reader.GetString(reader.GetOrdinal("folio"));

        if (!facturasDict.TryGetValue(folio, out var factura))
        {
            factura = new Factura
            {
                Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
                Fecha = reader.IsDBNull(reader.GetOrdinal("fecha")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("fecha")),
                Folio = folio,
                RutCliente = reader.IsDBNull(reader.GetOrdinal("rutcliente")) ? string.Empty : reader.GetString(reader.GetOrdinal("rutcliente")),
                Total = reader.IsDBNull(reader.GetOrdinal("total")) ? 0 : reader.GetInt32(reader.GetOrdinal("total")),
            };

            facturasDict[folio] = factura;
        }

        factura.DetalleFactura.Add(new Detalle
        {
            Id = reader.IsDBNull(reader.GetOrdinal("id")) ? 0 : reader.GetInt32(reader.GetOrdinal("id")),
            Folio = folio,
            Producto = reader.IsDBNull(reader.GetOrdinal("producto")) ? string.Empty : reader.GetString(reader.GetOrdinal("producto")),
            Cantidad = reader.IsDBNull(reader.GetOrdinal("total_cantidad")) ? 0 : reader.GetInt32(reader.GetOrdinal("total_cantidad")),
            Total = reader.IsDBNull(reader.GetOrdinal("total")) ? 0 : reader.GetInt32(reader.GetOrdinal("total"))
        });
    }

        return facturasDict.Values.ToList();
    }
}