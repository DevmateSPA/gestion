using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class ProveedorRepository : BaseRepository<Proveedor>, IProveedorRepository
{
    public ProveedorRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "proveedor", "vw_proveedor") {}

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
            .WhereEqual("empresa", empresaId)
            .WhereLike("rut", $"{busquedaRut}%")
            .ToListAsync<string>();
    }
}