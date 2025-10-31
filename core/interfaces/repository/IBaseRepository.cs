using Gestion.core.interfaces.model;

namespace Gestion.core.interfaces.repository;

public interface IBaseRepository<T> where T : IModel
{
    Task<T?> FindById(int id);
    Task<List<T>> FindAll();
    Task<bool> DeleteById(int id);
    Task<T> Save(T entity); // Actualiza y Guarda
}