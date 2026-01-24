using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Text;
using Gestion.core.attributes;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class FacturaCompraProductoRepository : BaseRepository<FacturaCompraProducto>, IFacturaCompraProductoRepository
{
    public FacturaCompraProductoRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "facturacompraproducto", null) {}

    public async Task<List<FacturaCompraProducto>> FindByFolio(string folio)
    {
        return await CreateQueryBuilder()
            .Where("folio = @folio AND tipo = 'FA'", new DbParam("@folio", folio))
            .ToListAsync<FacturaCompraProducto>();
    }

    public async Task<bool> SaveAll(IList<FacturaCompraProducto> detalles)
    {
        if (detalles == null || detalles.Count == 0)
            return false;

        // Propiedades válidas
        var props = typeof(FacturaCompraProducto).GetProperties()
            .Where(p => p.Name != "Id"
                        && !Attribute.IsDefined(p, typeof(NotMappedAttribute))
                        && !Attribute.IsDefined(p, typeof(NoSaveDbAttribute)))
            .ToList();

        string columns = string.Join(", ", props.Select(p => p.Name.ToLower()));

        // Construcción del INSERT múltiple
        var sb = new StringBuilder();
        sb.Append($"INSERT INTO {_tableName} ({columns}) VALUES ");

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

        int affected = await ExecuteNonQueryWithLogAsync("FacturaCompraProducto_SaveAll", sb.ToString(), parameters);
        return affected > 0;
    }

    public async Task<bool> UpdateAll(IList<FacturaCompraProducto> detalles)
    {
        if (detalles == null || detalles.Count == 0)
            return false;

        var props = typeof(FacturaCompraProducto).GetProperties()
            .Where(p => p.Name != "Id"
                        && !Attribute.IsDefined(p, typeof(NotMappedAttribute))
                        && !Attribute.IsDefined(p, typeof(NoSaveDbAttribute)))
            .ToList();

        // Construcción del UPDATE con CASE
        var sb = new StringBuilder();
        sb.Append($"UPDATE {_tableName} SET ");

        var parameters = new List<DbParam>();

        foreach (var prop in props)
        {
            string col = prop.Name.ToLower();
            sb.Append($"{col} = CASE id ");

            foreach (var entity in detalles)
            {
                var id = typeof(FacturaCompraProducto).GetProperty("Id")!.GetValue(entity)!;
                string paramName = $"@{col}_{id}";
                sb.Append($"WHEN {id} THEN {paramName} ");

                parameters.Add(new DbParam(paramName, prop.GetValue(entity) ?? DBNull.Value));
            }

            sb.Append("END");

            if (prop != props.Last())
                sb.Append(", ");
        }

        // WHERE id IN (...)
        var ids = detalles
            .Select(e => typeof(FacturaCompraProducto).GetProperty("Id")!.GetValue(e)!)
            .ToList();

        sb.Append($" WHERE id IN ({string.Join(", ", ids)})");

        int affected = await ExecuteNonQueryAsync(sb.ToString(), parameters);
        return affected > 0;
    }

    public async Task<bool> DeleteByIds(IList<long> ids)
    {
        if (ids == null || ids.Count == 0)
            return false;

        var parameters = ids.Select((id, i) => new DbParam($"@id{i}", id)).ToList();
        string paramList = string.Join(", ", parameters.Select(p => p.Name));

        string sql = $"DELETE FROM {_tableName} WHERE id IN ({paramList})";

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