using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;

namespace Gestion.Infrastructure.data;

public class ProveedorRepository : BaseRepository<Proveedor>, IProveedorRepository
{
    public ProveedorRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "proveedor", "vw_proveedor") {}

    public async Task<bool> ExisteRut(
        string rut,
        long empresaId,
        long? excludeId = null) => await ExistsByColumns(
            new Dictionary<string, object>
            {
                ["rut"] = rut,
                ["empresa"] = empresaId
            },
            excludeId);

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