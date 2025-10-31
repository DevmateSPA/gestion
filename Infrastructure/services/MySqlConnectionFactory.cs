using MySql.Data.MySqlClient;
using System.Data;

namespace Gestion.Infrastructure.Services;

public class MySqlConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory()
    {
        _connectionString = "Server=0.tcp.sa.ngrok.io;Port=12639;Database=imprenta;User ID=root;Password=12345;";
    }

    /// <summary>
    /// Crea y abre una conexión MySQL de forma asíncrona.
    /// </summary>
    public async Task<IDbConnection> CreateConnection()
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
