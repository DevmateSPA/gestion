using Gestion.core.interfaces;
using Gestion.core.model;
using Gestion.Infrastructure.Services;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public class ClienteRepository : IClienteRepository
{
    private readonly MySqlConnectionFactory _connectionFactory;

    public ClienteRepository(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Cliente>> GetClientes()
    {
        using var conn = await _connectionFactory.CreateConnection();
        const string sql = "SELECT * FROM cliente";

        using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);

        var clientes = new List<Cliente>();

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var cliente = new Cliente(
                rut: reader["rut"]?.ToString() ?? "Rut no disponible",
                razonSocial: reader["razon_social"]?.ToString() ?? "Razón social no disponible",
                giro: reader["giro"]?.ToString() ?? "Giro no disponible",
                direccion: reader["direccion"]?.ToString() ?? "Dirección no disponible",
                ciudad: reader["ciudad"]?.ToString() ?? "Ciudad no disponible",
                telefono: reader["telefono"]?.ToString() ?? "Telefono no disponible",
                fax: reader["fax"]?.ToString() ?? "Fax no disponible",
                obs1: reader["observaciones1"]?.ToString() ?? "Obs1 no disponible",
                obs2: reader["observaciones2"]?.ToString() ?? "Obs2 no disponible",
                debi: reader["debi"] != DBNull.Value ? Convert.ToInt32(reader["debi"]) : 0,
                habi: reader["habi"] != DBNull.Value ? Convert.ToInt32(reader["habi"]) : 0,
                debe: reader["debe"] != DBNull.Value ? Convert.ToInt64(reader["debe"]) : 0,
                habe: reader["habe"] != DBNull.Value ? Convert.ToInt64(reader["habe"]) : 0,
                saldo: reader["saldo"] != DBNull.Value ? Convert.ToInt64(reader["saldo"]) : 0
            );

            clientes.Add(cliente);
        }

        return clientes;
    }
}