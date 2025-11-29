using System.Data.Common;
using System.Reflection;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.model;
using Gestion.Infrastructure.Services;
using Gestion.core.interfaces.database;
using System.ComponentModel.DataAnnotations.Schema;
using Gestion.core.attributes;

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
            // Ignorar propiedades sin setter
            if (!prop.CanWrite)
                continue;

            // Ignorar propiedades [NotMapped]
            if (Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                continue;

            string col = prop.Name.ToLower();

            if (!reader.HasColumn(col))
                continue;

            object? value = reader[col];
            if (value == DBNull.Value)
                value = null;

            prop.SetValue(entity, value);
        }

        return entity;
    }

    public virtual async Task<T?> FindById(long id)
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

    public virtual async Task<List<T>> FindAll()
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

    public async Task<List<T>> FindWhereFrom(
        string tableOrView,
        string where,
        params DbParameter[] parameters)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        cmd.CommandText = $"SELECT * FROM {tableOrView} WHERE {where}";

        foreach (var p in parameters)
            cmd.Parameters.Add(p);

        using var reader = await cmd.ExecuteReaderAsync();

        var list = new List<T>();
        while (await reader.ReadAsync())
            list.Add(MapEntity(reader));

        return list;
    }

    public async Task<bool> DeleteById(long id)
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

    public virtual async Task<bool> Update(T entity)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        var props = typeof(T).GetProperties()
            .Where(p => p.Name != "Id" 
            && Attribute.IsDefined(p, typeof(NotMappedAttribute)) == false
            && !Attribute.IsDefined(p, typeof(DbIgnoreAttribute)))
            .ToList();

        var setClause = string.Join(", ", props.Select(p => $"{p.Name.ToLower()} = @{p.Name.ToLower()}"));

        cmd.CommandText = $"UPDATE {_tableName} SET {setClause} WHERE id = @id";

        foreach (var prop in props)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = $"@{prop.Name.ToLower()}";
            param.Value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }

        var idParam = cmd.CreateParameter();
        idParam.ParameterName = "@id";
        idParam.Value = typeof(T).GetProperty("Id")?.GetValue(entity) ?? DBNull.Value;
        cmd.Parameters.Add(idParam);

        int affected = await cmd.ExecuteNonQueryAsync();
        return affected > 0;
    }

    public virtual async Task<bool> Save(T entity)
    {
        using var conn = await _connectionFactory.CreateConnection();
        using var cmd = (DbCommand)conn.CreateCommand();

        var props = typeof(T).GetProperties()
            .Where(p => p.Name != "Id" 
            && Attribute.IsDefined(p, typeof(NotMappedAttribute)) == false
            && !Attribute.IsDefined(p, typeof(DbIgnoreAttribute)))
            .ToList();

        var columns = string.Join(", ", props.Select(p => p.Name.ToLower()));
        var parameters = string.Join(", ", props.Select(p => $"@{p.Name.ToLower()}"));

        cmd.CommandText = $"INSERT INTO {_tableName} ({columns}) VALUES ({parameters})";

        foreach (var prop in props)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = $"@{prop.Name.ToLower()}";
            param.Value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.Add(param);
        }

        int affected = await cmd.ExecuteNonQueryAsync();
        return affected > 0;
    }
    public abstract Task<List<T>> FindAllByEmpresa(long empresaId);
}