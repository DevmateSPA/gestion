using System.IO;
using dotenv.net;

namespace Gestion.Infrastructure.Services;
public static class DatabaseConfig
{
    static DatabaseConfig()
    {
        // Intenta primero con el directorio base (bin/Debug o bin/Release)
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var envPath = Path.Combine(basePath, ".env");

        // Si no existe ahí, intenta subir hasta encontrarlo
        if (!File.Exists(envPath))
        {
            var dir = new DirectoryInfo(basePath);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, ".env")))
                dir = dir.Parent;

            if (dir != null)
                envPath = Path.Combine(dir.FullName, ".env");
        }

        if (!File.Exists(envPath))
            throw new FileNotFoundException($".env no encontrado en {basePath} ni en carpetas superiores.");

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