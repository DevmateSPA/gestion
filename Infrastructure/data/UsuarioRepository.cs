using System.Data.Common;
using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.core.model.DTO;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "usuario", "vw_usuario") {}

    public override async Task<List<Usuario>> FindAllByEmpresa(long empresaId)
    {
        using var conn = await _connectionFactory.CreateConnection();

        string sql = $"""
            SELECT 
                *
            FROM {_viewName}
            WHERE empresa = @empresa
            ORDER BY fecha DESC;
            """;

        using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);
        cmd.Parameters.Add("@empresa", MySqlDbType.Int64).Value = empresaId;

        using var reader = await cmd.ExecuteReaderAsync();

        var usuarios = new List<Usuario>();

        while (await reader.ReadAsync())
        {
            usuarios.Add(new Usuario
            {
                Id        = reader.GetInt64(reader.GetOrdinal("id")),
                Nombre    = reader.GetString(reader.GetOrdinal("nombre")),
                Clave     = reader.GetString(reader.GetOrdinal("clave")),
                Empresa   = reader.GetInt64(reader.GetOrdinal("empresa")),
                Tipo      = reader.GetInt64(reader.GetOrdinal("tipo")),
                TipoDesc  = reader.GetString(reader.GetOrdinal("TipoDesc"))
            });
        }

        return usuarios;
    }

    public async Task<Usuario?> GetByNombre(string nombreUsuario, long empresaId)
    {
        using var conn = await _connectionFactory.CreateConnection();

        string sql = $"""
            SELECT 
                *
            FROM {_viewName}
            WHERE empresa = @empresa and LOWER(nombre) = @nombre;
            """;

        using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);
        cmd.Parameters.Add("@nombre", MySqlDbType.VarChar, 100).Value = nombreUsuario.ToLower();
        cmd.Parameters.Add("@empresa", MySqlDbType.Int64).Value = empresaId;

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            Usuario usuario = new()
            {
                Id        = reader.GetInt64(reader.GetOrdinal("id")),
                Nombre    = reader.GetString(reader.GetOrdinal("nombre")),
                Clave     = reader.GetString(reader.GetOrdinal("clave")),
                Empresa   = reader.GetInt64(reader.GetOrdinal("empresa")),
                Tipo      = reader.GetInt64(reader.GetOrdinal("tipo")),
                TipoDesc  = reader.GetString(reader.GetOrdinal("tipodesc"))
            };

            return usuario;
        }
        return null;
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