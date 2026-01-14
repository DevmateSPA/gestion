using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
{
    public ClienteRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "cliente", "vw_cliente") {}

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

    public async Task<List<string>> GetRutList(string busquedaRut, long empresaId)
    {
        if (_viewName == null)
            throw new InvalidOperationException("La vista no está asignada para este repositorio.");

        if (string.IsNullOrWhiteSpace(busquedaRut) || busquedaRut.Length < 1)
            return [];

        var busquedaParam = $"{busquedaRut}%";

        DbParameter[] parameters =
        [
            new MySqlParameter("@busquedaParam", busquedaParam),
            new MySqlParameter("@empresa", empresaId),
        ];

        return await GetColumnList<string>(
            columnName: "rut", 
            where: "empresa = @empresa AND rut LIKE @busquedaParam", 
            parameters: parameters);
    }
}