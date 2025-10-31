using Gestion.core.interfaces.database;
using MySql.Data.MySqlClient;
using System.Data;

namespace Gestion.Infrastructure.Services;

public class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory()
    {
        _connectionString = DatabaseConfig.GetConnectionString();
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
