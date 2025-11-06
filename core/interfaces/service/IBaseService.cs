namespace Gestion.core.interfaces.service;

public interface IBaseService<T>
{
    Task<T?> FindById(int id);
    Task<List<T>> FindAll();
    Task<bool> DeleteById(int id);
    Task<T> Update(T entity);
}