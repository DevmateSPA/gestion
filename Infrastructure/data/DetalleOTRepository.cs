using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Text;
using Gestion.core.attributes;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model.detalles;

namespace Gestion.Infrastructure.data;

public class DetalleOTRepository : BaseRepository<DetalleOrdenTrabajo>, IDetalleOTRepository
{
    public DetalleOTRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "ordentrabajodetalle", null) {}

    public async Task<List<DetalleOrdenTrabajo>> FindByFolio(string folio)
    {
        return await CreateQueryBuilder()
            .Where("folio = @folio", new DbParam("@folio", folio))
            .ToListAsync<DetalleOrdenTrabajo>();
    }

    public async Task<bool> SaveAll(IList<DetalleOrdenTrabajo> detalles)
    {
        if (detalles == null || detalles.Count == 0)
            return false;

        var props = typeof(DetalleOrdenTrabajo).GetProperties()
            .Where(p => p.Name != "Id"
                        && !Attribute.IsDefined(p, typeof(NotMappedAttribute))
                        && !Attribute.IsDefined(p, typeof(NoSaveDbAttribute)))
            .ToList();

        string columns = string.Join(", ", props.Select(p => p.Name.ToLower()));

        var sb = new StringBuilder();
        var parameters = new List<DbParam>();

        for (int i = 0; i < detalles.Count; i++)
        {
            var paramNames = new List<string>();

            foreach (var prop in props)
            {
                string paramName = $"@{prop.Name}_{i}";
                paramNames.Add(paramName);
                parameters.Add(new DbParam(paramName, prop.GetValue(detalles[i]) ?? DBNull.Value));
            }

            sb.Append("(" + string.Join(", ", paramNames) + ")");
            if (i < detalles.Count - 1)
                sb.Append(", ");
        }

        string sql = $"INSERT INTO {_tableName} ({columns}) VALUES {sb}";
        int affected = await ExecuteNonQueryAsync(sql, parameters);

        return affected > 0;
    }

    public async Task<bool> UpdateAll(IList<DetalleOrdenTrabajo> detalles)
    {
        if (detalles == null || detalles.Count == 0)
            return false;

        var props = typeof(DetalleOrdenTrabajo).GetProperties()
            .Where(p => p.Name != "Id"
                        && !Attribute.IsDefined(p, typeof(NotMappedAttribute))
                        && !Attribute.IsDefined(p, typeof(NoSaveDbAttribute)))
            .ToList();

        var sb = new StringBuilder();
        var parameters = new List<DbParam>();

        sb.Append($"UPDATE {_tableName} SET ");

        foreach (var prop in props)
        {
            string col = prop.Name.ToLower();
            sb.Append($"{col} = CASE id ");

            foreach (var entity in detalles)
            {
                var id = typeof(DetalleOrdenTrabajo).GetProperty("Id")!.GetValue(entity)!;
                string paramName = $"@{col}_{id}";

                sb.Append($"WHEN {id} THEN {paramName} ");
                parameters.Add(new DbParam(paramName, prop.GetValue(entity) ?? DBNull.Value));
            }

            sb.Append("END");

            if (prop != props.Last())
                sb.Append(", ");
        }

        var ids = detalles.Select(d => typeof(DetalleOrdenTrabajo).GetProperty("Id")!.GetValue(d)!).ToList();
        sb.Append($" WHERE id IN ({string.Join(", ", ids)})");

        int affected = await ExecuteNonQueryAsync(sb.ToString(), parameters);

        return affected > 0;
    }

    public async Task<bool> DeleteByIds(IList<long> ids)
    {
        if (ids == null || ids.Count == 0)
            return false;

        var parameters = ids.Select((id, i) => new DbParam($"@id{i}", id)).ToList();
        string sql = $"DELETE FROM {_tableName} WHERE id IN ({string.Join(", ", parameters.Select(p => p.Name))})";

        int affected = await ExecuteNonQueryAsync(sql, parameters);

        return affected > 0;
    }

    public async Task<bool> DeleteByFolio(string folio)
    {
        if (string.IsNullOrWhiteSpace(folio))
            return false;

        string sql = $"DELETE FROM {_tableName} WHERE folio = @folio";
        int affected = await ExecuteNonQueryAsync(sql, [new DbParam("@folio", folio)]);

        return affected > 0;
    }
}