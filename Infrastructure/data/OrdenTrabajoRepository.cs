using System.Collections.ObjectModel;
using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.core.model.detalles;
using Gestion.core.session;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class OrdenTrabajoRepository : BaseRepository<OrdenTrabajo>, IOrdenTrabajoRepository
{
    public OrdenTrabajoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "otprueba") {}

    public override async Task<List<OrdenTrabajo>> FindAllByEmpresa(long empresaId)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = $@"
            SELECT
                f.id,
                f.folio,
                f.fecha,
                f.rutcliente,
                f.descripcion,
                f.cantidad,
                f.totalimpresion,
                f.foliodesde,
                f.foliohasta,
                f.cortartamanio,
                f.cortartamanion,
                f.cortartamaniolargo,
                f.montar,
                f.moldetamanio,
                f.tamaniofinalancho,
                f.tamaniofinallargo,
                f.clienteproporcionapelicula,
                f.clienteproporcionaplancha,
                f.clienteproporcionapapel,
                f.tipoimpresion,
                f.maquina1,
                f.maquina2,
                f.pin,
                f.nva,
                f.us,
                f.ctpnva,
                f.u,
                f.sobres,
                f.sacos,
                f.tintas1,
                f.tintas2,
                f.tintas3,
                f.tintas4
            FROM {_tableName} f
            WHERE
                f.empresa = {SesionApp.IdEmpresa}";

        using var reader = await cmd.ExecuteReaderAsync();

        var ots = new Dictionary<long, OrdenTrabajo>();

        while (await reader.ReadAsync())
        {
            long otId = reader.GetInt64(reader.GetOrdinal("id"));

            if (!ots.TryGetValue(otId, out var ot))
            {
                ot = MapEntity(reader);
                ot.Detalles = new ObservableCollection<DetalleOrdenTrabajo>();
                ots.Add(otId, ot);
            }
        }

        return [.. ots.Values];
    }
}