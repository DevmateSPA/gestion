using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.core.model.detalles;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class DetalleOTRepository : BaseRepository<DetalleOrdenTrabajo>, IDetalleOTRepository
{
    public DetalleOTRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "ordentrabajodetalle") {}

    public override Task<List<DetalleOrdenTrabajo>> FindAllByEmpresa(long empresaId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<DetalleOrdenTrabajo>> FindByFolio(string folio)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {_tableName} WHERE folio = @folio";

        var param = cmd.CreateParameter();
        param.ParameterName = "@folio";
        param.Value = folio;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        var list = new List<DetalleOrdenTrabajo>();
        while (await reader.ReadAsync())
            list.Add(MapEntity(reader));

        return list;
    }
}