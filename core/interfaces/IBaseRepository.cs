using Gestion.Infrastructure.Services;

namespace Gestion.core.interfaces;

public interface IBaseRepository<T> where T : IModel
{
    Task<T?> FindById(int id);
    Task<bool> DeteleById(int id);
    Task<T> Save(T entity); // Actualiza y Guarda
}