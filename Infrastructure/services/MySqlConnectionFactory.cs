using MySql.Data.MySqlClient;
using System.Data;
using System.Threading.Tasks;

namespace Gestion.Infrastructure.Services;

public class MySqlConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Crea y abre una conexión MySQL de forma asíncrona.
    /// </summary>
    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
