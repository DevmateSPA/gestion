using Gestion.core.interfaces.repository;
using Gestion.core.model;
using Gestion.Infrastructure.Services;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly MySqlConnectionFactory _connectionFactory;

    public UsuarioRepository(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Usuario?> GetByNombre(string nombreUsuario)
    {
        using var conn = await _connectionFactory.CreateConnection();
        const string sql = "SELECT id, nombre, contraseña FROM usuario_pruebas WHERE nombre = @nombre";

        using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);
        cmd.Parameters.Add("@nombre", MySqlDbType.VarChar, 100).Value = nombreUsuario;

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return new Usuario(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));

        return null;
    }
}