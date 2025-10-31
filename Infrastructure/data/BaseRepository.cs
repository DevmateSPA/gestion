using System.Data.Common;
using System.Reflection;
using Gestion.core.interfaces;
using Gestion.Infrastructure.Services;
using MySql.Data.MySqlClient;

namespace Gestion.Infrastructure.data;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : IModel, new()
{
    protected readonly MySqlConnectionFactory _connectionFactory;
    protected readonly string _tableName;

    protected BaseRepository(MySqlConnectionFactory connectionFactory, string tableName)
    {
        _connectionFactory = connectionFactory;
        _tableName = tableName;
    }

    protected virtual T MapEntity(DbDataReader reader)
    {
        var entity = new T();

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            if (!reader.HasColumn(prop.Name)) 
                continue;

            object? value = reader[prop.Name];
            if (value == DBNull.Value)
                value = null;

            prop.SetValue(entity, value);
        }

        return entity;
    }

    public async Task<T?> FindById(int id)
    {
        using var conn = await _connectionFactory.CreateConnection();
        string sql = $"SELECT * FROM {_tableName} WHERE id = @id";

        using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapEntity(reader);

        return default;
    }

    public async Task<bool> DeteleById(int id)
    {
        using var conn = await _connectionFactory.CreateConnection();
        string sql = $"DELETE FROM {_tableName} WHERE id = @id";

        using var cmd = new MySqlCommand(sql, (MySqlConnection)conn);
        cmd.Parameters.AddWithValue("@id", id);

        int affected = await cmd.ExecuteNonQueryAsync();
        return affected > 0;
    }

    // Cada Repositorio lo implementara
    public abstract Task<T> Save(T entity);
}