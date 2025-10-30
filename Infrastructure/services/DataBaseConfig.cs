using dotenv.net;

namespace Gestion.Infrastructure.Services;
public static class DatabaseConfig
{
    static DatabaseConfig()
    {
        DotEnv.Load();
    }

    private static string _getEnvVariable(string variable)
    {
        return Environment.GetEnvironmentVariable(variable)
            ?? throw new InvalidOperationException($"{variable} no configurada");
    }

    public static string GetConnectionString()
    {
        string host = _getEnvVariable("DB_HOST");
        string db = _getEnvVariable("DB_NAME");
        string user = _getEnvVariable("DB_USER");
        string password = _getEnvVariable("DB_PASSWORD");

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(db))
            throw new Exception("Faltan variables de entorno para la base de datos.");

        return $"Server={host};Database={db};User ID={user};Password={password};";
    }
}