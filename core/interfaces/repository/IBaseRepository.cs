using System.Data.Common;
using Gestion.core.interfaces.model;

namespace Gestion.core.interfaces.repository;

public interface IBaseRepository<T> where T : IModel
{
    Task<T?> FindById(long id);
    Task<List<T>> FindAll();
    Task<List<T>> FindWhereFrom(string tableOrView,
        string where,
        string? orderBy,
        int? limit = null,
        int? offset = null,
        params DbParameter[] parameters);
    Task<bool> DeleteById(long id);
    Task<bool> Update(T entity); // Actualiza
    Task<bool> Save(T entity);   // Crea
    Task<List<T>> FindAllByEmpresa(long empresaId);
    Task<List<T>> FindPageByEmpresa(long empresaId, int pageNumber, int pageSize);
    Task<List<T>> FindPageWhere(string where, string? orderBy, int pageNumber, int pageSize, params DbParameter[] parameters);
    Task<long> CountWhere(string where, params DbParameter[] parameters);
}