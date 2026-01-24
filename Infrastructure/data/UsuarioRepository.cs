using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.core.model.DTO;

namespace Gestion.Infrastructure.data;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "usuario", "vw_usuario") {}

    public override async Task<List<Usuario>> FindAllByEmpresa(long empresaId)
    {
        return await CreateQueryBuilder()
            .Where("empresa = @empresa", new DbParam("@empresa", empresaId))
            .OrderBy("fecha DESC")
            .ToListAsync<Usuario>();
    }

    public async Task<Usuario?> GetByNombre(string nombreUsuario, long empresaId)
    {
        var list = await CreateQueryBuilder()
            .Where("empresa = @empresa AND LOWER(nombre) = @nombre",
                new DbParam("@empresa", empresaId),
                new DbParam("@nombre", nombreUsuario.ToLower()))
            .Limit(1)
            .ToListAsync<Usuario>();

        return list.FirstOrDefault();
    }

    public async Task<List<TipoUsuarioDTO>> GetTipoList()
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = """
            SELECT
                id_tipo,
                nombre_tipo
            FROM usuario_tipo;
            """;

        var list = new List<TipoUsuarioDTO>();

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new TipoUsuarioDTO
            {
                Id = Convert.ToInt64(reader["id_tipo"]),
                Nombre = Convert.ToString(reader["nombre_tipo"])!
            });
        }
        return list;
    }
}