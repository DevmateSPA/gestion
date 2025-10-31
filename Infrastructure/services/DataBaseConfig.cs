using System.IO;
using dotenv.net;

namespace Gestion.Infrastructure.Services;
public static class DatabaseConfig
{
    static DatabaseConfig()
    {
        // Ruta explícita al archivo .env en la raíz del proyecto
        string projectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
        string envPath = Path.GetFullPath(Path.Combine(projectRoot, ".env"));

        if (!File.Exists(envPath))
            throw new FileNotFoundException($".env no encontrado en {envPath}");

        DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { envPath }));
    }

    private static string _getEnvVariable(string variable)
    {
        return Environment.GetEnvironmentVariable(variable)
            ?? throw new InvalidOperationException($"{variable} no configurada");
    }

    public static string GetConnectionString()
    {
        string host = _getEnvVariable("DB_HOST");
        string port = _getEnvVariable("DB_PORT");
        string db = _getEnvVariable("DB_NAME");
        string user = _getEnvVariable("DB_USER");
        string password = _getEnvVariable("DB_PASSWORD");

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(db))
            throw new Exception("Faltan variables de entorno para la base de datos.");

        return $"Server={host};Port={port};Database={db};User ID={user};Password={password};";
    }
}