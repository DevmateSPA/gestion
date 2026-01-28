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

        return await CreateQueryBuilder()
            .Select("rut")
            .Where("empresa = @empresa AND rut LIKE @busquedaParam",
                new DbParam("@empresa", empresaId),
                new DbParam("@busquedaParam", $"{busquedaRut}%"))
            .ToListAsync<string>();
    }
}