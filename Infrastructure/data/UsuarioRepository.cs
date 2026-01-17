using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "usuario", "usuario") {}

    public async Task<Usuario?> GetByNombre(string nombreUsuario)
    {
        using var conn = await _connectionFactory.CreateConnection();
        string sql = $"SELECT id, nombre, clave, tipo FROM {_tableName} WHERE lower(nombre) = @nombre";

        using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);
        cmd.Parameters.Add("@nombre", MySqlDbType.VarChar, 100).Value = nombreUsuario.ToLower();

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            Usuario usuario = new Usuario()
            {
                Id = reader.GetInt64(0),
                Nombre = reader.GetString(1),
                Clave = reader.GetString(2),
                Tipo = reader.GetString(3)
            };

            return usuario;
        }

        return null;
    }
}