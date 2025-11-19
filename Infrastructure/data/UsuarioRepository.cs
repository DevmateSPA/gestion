using Gestion.core.interfaces.database;
using Gestion.core.interfaces.repository;
using Gestion.core.model;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(IDbConnectionFactory connectionFactory)
        : base(connectionFactory, "usuario_pruebas") {}

    public async Task<Usuario?> GetByNombre(string nombreUsuario)
    {
        using var conn = await _connectionFactory.CreateConnection();
        string sql = $"SELECT id, nombre, contraseña FROM {_tableName} WHERE nombre = @nombre";

        using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);
        cmd.Parameters.Add("@nombre", MySqlDbType.VarChar, 100).Value = nombreUsuario;

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return new Usuario(reader.GetInt64(0), reader.GetString(1), reader.GetString(2));

        return null;
    }
}