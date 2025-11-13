using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class DetalleRepository : BaseRepository<Detalle>, IDetalleRepository
{
    public DetalleRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "facturadetalle") {}

    public async Task<List<Detalle>> FindByFolio(string folio)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {_tableName} WHERE folio = @folio";

        var param = cmd.CreateParameter();
        param.ParameterName = "@folio";
        param.Value = folio;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();

        var entities = new List<Detalle>();
        while (await reader.ReadAsync())
        {
            var detalle = new Detalle
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Producto = reader["producto"]?.ToString() ?? string.Empty,
                Precio = reader.GetInt32(reader.GetOrdinal("precio")),
                Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                Total = reader.GetInt32(reader.GetOrdinal("total")),
                Folio = reader["folio"]?.ToString() ?? string.Empty,
                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha"))
            };

            entities.Add(detalle);
        }

        return entities;
    }
}