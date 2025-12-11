using System.Data.Common;
using Gestion.core.interfaces.model;

namespace Gestion.core.interfaces.repository;

public interface IBaseRepository<T> where T : IModel
{
    Task<T?> FindById(long id);
    Task<List<T>> FindAll();
    Task<List<T>> FindWhereFrom(string tableOrView,
        string where, 
        int? limit = null,
        int? offset = null,
        params DbParameter[] parameters);
    Task<bool> DeleteById(long id);
    Task<bool> Update(T entity); // Actualiza
    Task<bool> Save(T entity);   // Crea
    Task<List<T>> FindAllByEmpresa(long empresaId);
    Task<long> CountWhere(string where, params DbParameter[] parameters);
}