using System.Data.Common;
using System.Reflection;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.model;
using Gestion.Infrastructure.Services;
using Gestion.core.interfaces.database;

namespace Gestion.Infrastructure.data;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : IModel, new()
{
    protected readonly IDbConnectionFactory _connectionFactory;
    protected readonly string _tableName;

    protected BaseRepository(IDbConnectionFactory connectionFactory, string tableName)
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
        using var cmd = (DbCommand)conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {_tableName} WHERE id = @id";

        var param = cmd.CreateParameter();
        param.ParameterName = "@id";
        param.Value = id;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
            return MapEntity(reader);

        return default;
    }

    public async Task<List<T>> FindAll()
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {_tableName}";

        using var reader = await cmd.ExecuteReaderAsync();

        var entities = new List<T>();
        while (await reader.ReadAsync())
        {
            entities.Add(MapEntity(reader));
        }

        return entities;
    }

    public async Task<bool> DeleteById(int id)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();
        cmd.CommandText = $"DELETE FROM {_tableName} WHERE id = @id";

        var param = cmd.CreateParameter();
        param.ParameterName = "@id";
        param.Value = id;
        cmd.Parameters.Add(param);

        int affected = await cmd.ExecuteNonQueryAsync();
        return affected > 0;
    }

    // Cada Repositorio lo implementara
    public abstract Task<T> Save(T entity);
}