using System.ComponentModel.DataAnnotations.Schema;
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

    public async Task<List<FacturaCompraProducto>> FindByFolio(string folio, long empresaId)
    {
        return await CreateQueryBuilder()
            .Where("folio = @folio", new DbParam("@folio", folio))
            .Where("empresa = @empresa", new DbParam("@empresa", empresaId))
            .ToListAsync<FacturaCompraProducto>();
    }

    public async Task<bool> SaveAll(IList<FacturaCompraProducto> detalles)
    {
        if (detalles == null || detalles.Count == 0)
            return false;

        var props = typeof(FacturaCompraProducto).GetProperties()
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

        int affected = await ExecuteNonQueryWithLogAsync(
            operation: "[FacturaCompraProducto_SaveAll]",
            sql: sql,
            parameters: parameters
        );

        return affected > 0;
    }

    public async Task<bool> UpdateAll(IList<FacturaCompraProducto> detalles, long empresaId)
    {
        if (detalles == null || detalles.Count == 0)
            return false;

        var props = typeof(FacturaCompraProducto).GetProperties()
            .Where(p => 
                        p.Name != "Id"
                        && p.Name != "Empresa"
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
                var id = typeof(FacturaCompraProducto).GetProperty("Id")!.GetValue(entity)!;
                string paramName = $"@{col}_{id}";

                sb.Append($"WHEN {id} THEN {paramName} ");
                parameters.Add(new DbParam(paramName, prop.GetValue(entity) ?? DBNull.Value));
            }

            sb.Append("END");

            if (prop != props.Last())
                sb.Append(", ");
        }


        var ids = detalles.Select(d => typeof(FacturaCompraProducto).GetProperty("Id")!.GetValue(d)!).ToList();
        sb.Append($" WHERE id IN ({string.Join(", ", ids)}) AND empresa = @empresa");

        parameters.Add(new DbParam("@empresa", empresaId));

        int affected = await ExecuteNonQueryWithLogAsync(
            operation: "[FacturaCompraProducto_UpdateAll]",
            sql: sb.ToString(),
            parameters: parameters
        );

        return affected > 0;
    }

    public async Task<bool> DeleteByIds(IList<long> ids, long empresaId)
    {
        if (ids == null || ids.Count == 0)
            return false;

        var parameters = ids.Select((id, i) => new DbParam($"@id{i}", id)).ToList();
        string sql = $@"DELETE FROM {_tableName} 
            WHERE id IN ({string.Join(", ", parameters.Select(p => p.Name))}) 
            AND empresa = @empresa";

        parameters.Add(new DbParam("@empresa", empresaId));

        int affected = await ExecuteNonQueryWithLogAsync(
            operation: "[FacturaCompraProducto_DeleteByIds]",
            sql: sql,
            parameters: parameters
        );

        return affected > 0;
    }

    public async Task<bool> DeleteByFolio(string folio, long empresaId)
    {
        if (string.IsNullOrWhiteSpace(folio))
            return false;

        int affected = await ExecuteNonQueryWithLogAsync(
            operation: "[FacturaCompraProducto_DeleteByFolio]",
            sql: $"DELETE FROM {_tableName} WHERE folio = @folio AND empresa = @empresa",
            parameters:
            [
                new DbParam("@folio", folio),
                new DbParam("@empresa", empresaId)
            ]
        );

        return affected > 0;
    }
}