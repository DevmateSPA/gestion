using Gestion.core.interfaces.database;
using Gestion.core.interfaces.service;
using MySql.Data.MySqlClient;
using System.Data;

namespace Gestion.Infrastructure.Services;

public class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly IDialogService _dialogService;
    private readonly string _connectionString;

    public MySqlConnectionFactory(IDialogService dialogService)
    {
        _dialogService = dialogService;

        try
        {
            // Intenta obtener la cadena de conexión desde las variables de entorno
            _connectionString = DatabaseConfig.GetConnectionString();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Error al obtener la cadena de conexión: {ex.Message}");
            _connectionString = string.Empty;
        }
    }

    public async Task<IDbConnection> CreateConnection()
    {
        if (string.IsNullOrEmpty(_connectionString))
            throw new InvalidOperationException("La cadena de conexión no está configurada correctamente.");

        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
