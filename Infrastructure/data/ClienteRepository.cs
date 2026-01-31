using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "cliente", "vw_cliente") {}

    public async Task<List<string>> GetRutList(
        string busquedaRut, 
        long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        if (string.IsNullOrWhiteSpace(busquedaRut) || busquedaRut.Length < 1)
            return [];

        var rutNormalizado = busquedaRut
            .Replace(".", "")
            .Replace("-", "")
            .Trim();

        return await CreateQueryBuilder()
            .Select("rut")
            .WhereEqual("empresa", empresaId)
            .Where(
                "REPLACE(REPLACE(TRIM(rut), '-', ''), '.', '') LIKE @rut",
                 new DbParam("@rut", $"{rutNormalizado}%"))
            .ToListAsync<string>();
    }

    public async Task<Cliente?> FindByRut(
        string rut,
        long empresaId)
    {
        var result = await CreateQueryBuilder()
            .WhereEqual("rut", rut)
            .WhereEqual("empresa", empresaId)
            .Limit(1)
            .ToListAsync<Cliente>();

        return result.FirstOrDefault();
    }
}