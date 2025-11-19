using Gestion.core.interfaces.model;

namespace Gestion.core.interfaces.repository;

public interface IBaseRepository<T> where T : IModel
{
    Task<T?> FindById(long id);
    Task<List<T>> FindAll();
    Task<bool> DeleteById(long id);
    Task<bool> Update(T entity); // Actualiza
    Task<bool> Save(T entity);   // Crea
}