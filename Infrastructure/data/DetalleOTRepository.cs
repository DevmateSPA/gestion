using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Text;
using Gestion.core.attributes;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.core.model.detalles;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class DetalleOTRepository : BaseRepository<DetalleOrdenTrabajo>, IDetalleOTRepository
{
    public DetalleOTRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "ordentrabajodetalle", null) {}

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

    public async Task<bool> SaveAll(IList<DetalleOrdenTrabajo> detalles)
    {
        var lista = detalles.ToList();
        if (lista.Count == 0)
            return false;

        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        // --- PROPIEDADES VÁLIDAS ---
        var props = typeof(DetalleOrdenTrabajo).GetProperties()
            .Where(p => p.Name != "Id" 
                && Attribute.IsDefined(p, typeof(NotMappedAttribute)) == false
                && !Attribute.IsDefined(p, typeof(DbIgnoreAttribute)))
            .ToList();

        string columns = string.Join(", ", props.Select(p => p.Name.ToLower()));

        // --- CONSTRUCCIÓN DEL INSERT MÚLTIPLE ---
        var sb = new StringBuilder();
        sb.Append($"INSERT INTO {_tableName} ({columns}) VALUES ");

        int index = 0;

        foreach (var entity in lista)
        {
            var paramNames = new List<string>();

            foreach (var prop in props)
            {
                string paramName = $"@{prop.Name.ToLower()}{index}";
                paramNames.Add(paramName);

                var param = cmd.CreateParameter();
                param.ParameterName = paramName;
                param.Value = prop.GetValue(entity) ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }

            sb.Append("(" + string.Join(", ", paramNames) + ")");

            index++;
            if (index < lista.Count)
                sb.Append(", ");
        }

        cmd.CommandText = sb.ToString();

        int affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0;
    }

    public async Task<bool> UpdateAll(IList<DetalleOrdenTrabajo> detalles)
    {
        var lista = detalles.ToList();

        if (lista.Count == 0)
            return false;

        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        var props = typeof(DetalleOrdenTrabajo).GetProperties()
            .Where(p => p.Name != "Id"
                && !Attribute.IsDefined(p, typeof(NotMappedAttribute))
                && !Attribute.IsDefined(p, typeof(DbIgnoreAttribute)))
            .ToList();

        var sb = new StringBuilder();
        sb.Append($"UPDATE {_tableName} SET ");

        // Construcción de columnas con CASE
        foreach (var prop in props)
        {
            string col = prop.Name.ToLower();
            sb.Append($"{col} = CASE id ");

            foreach (var entity in lista)
            {
                // Es tipo long pero no quiero castear
                var id = typeof(DetalleOrdenTrabajo).GetProperty("Id")!.GetValue(entity)!;
                string paramName = $"@{col}{id}";

                sb.Append($"WHEN {id} THEN {paramName} ");

                var param = cmd.CreateParameter();
                param.ParameterName = paramName;
                param.Value = prop.GetValue(entity) ?? DBNull.Value;
                cmd.Parameters.Add(param);
            }

            sb.Append("END");

            if (prop != props.Last())
                sb.Append(", ");
        }

        // WHERE id IN (...)
        var ids = lista
            .Select(e => typeof(DetalleOrdenTrabajo).GetProperty("Id")!.GetValue(e)!)
            .ToList();

        sb.Append($" WHERE id IN ({string.Join(", ", ids)})");

        cmd.CommandText = sb.ToString();

        int affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0;
    }

    public async Task<bool> DeleteByIds(IList<long> ids)
    {
        if (ids == null || ids.Count == 0)
            return false;

        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        var parameters = new List<string>();

        for (int i = 0; i < ids.Count; i++)
        {
            string paramName = $"@id{i}";
            parameters.Add(paramName);

            var param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.Value = ids[i];
            cmd.Parameters.Add(param);
        }

        cmd.CommandText = $"DELETE FROM {_tableName} WHERE id IN ({string.Join(", ", parameters)})";

        int affected = await cmd.ExecuteNonQueryAsync();

        return affected > 0;
    }

    public async Task<bool> DeleteByFolio(string folio)
    {
        if (string.IsNullOrWhiteSpace(folio))
            return false;

        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = $"DELETE FROM {_tableName} WHERE folio = @folio";

        var p = cmd.CreateParameter();
        p.ParameterName = "@folio";
        p.Value = folio;
        cmd.Parameters.Add(p);

        int affected = await cmd.ExecuteNonQueryAsync();

        return true;
    }
}