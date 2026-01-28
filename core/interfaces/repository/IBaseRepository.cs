using System.Data.Common;
using Gestion.core.interfaces.model;
using Gestion.Infrastructure.data;

namespace Gestion.core.interfaces.repository;

public interface IBaseRepository<T> where T : IModel, new()
{
    Task<T?> FindById(long id);
    Task<List<T>> FindAll();
    Task<List<T>> FindWhereFrom(
        string tableOrView,
        string where,
        string? orderBy,
        int? limit = null,
        int? offset = null,
        IEnumerable<DbParam>? parameters = null,
        string selectColumns = "*");
    Task<bool> DeleteById(long id);
    Task<bool> Update(T entity); // Actualiza
    Task<bool> Save(T entity);   // Crea
    Task<List<T>> FindAllByEmpresa(long empresaId);
    Task<List<T>> FindPageByEmpresa(
        long empresaId, 
        int pageNumber, 
        int pageSize);
    Task<long> CountWhere(
        string where, 
        string? tableName, 
        IEnumerable<DbParam>? parameters = null);
    Task<long> ContarPorEmpresa(long empresaId);
    Task<List<TData>> GetColumnList<TData>(
        string columnName, 
        string where, 
        IEnumerable<DbParam>? parameters = null);

    Task<bool> ExistsByColumns(
        IEnumerable<(string column, object value)> columns,
        long? excludeId = null);

    Task<bool> Exists(
        Action<QueryBuilder<T>> build,
        long? excludeId = null);
}