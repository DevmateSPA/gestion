using System.IO;
using System.Windows;
using dotenv.net;

namespace Gestion.Infrastructure.Services;
public static class DatabaseConfig
{
static DatabaseConfig()
{
    try
    {
        string projectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..");
        string envPath = Path.GetFullPath(Path.Combine(projectRoot, ".env"));

        if (File.Exists(envPath))
        {
            DotEnv.Load(new DotEnvOptions(envFilePaths: new[] { envPath }));
            Console.WriteLine($".env cargado desde {envPath}");
        }
        else
        {
            Console.WriteLine($".env no encontrado, usando valores por defecto.");
            Environment.SetEnvironmentVariable("DB_HOST", "0.tcp.sa.ngrok.io");
            Environment.SetEnvironmentVariable("DB_PORT", "19003");
            Environment.SetEnvironmentVariable("DB_USER", "root");
            Environment.SetEnvironmentVariable("DB_PASSWORD", "Devmate.2025@");
            Environment.SetEnvironmentVariable("DB_NAME", "imprenta");
        }
    }
    catch (Exception ex)
    {
            MessageBox.Show(ex.Message);
    }
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