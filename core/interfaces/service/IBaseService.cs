using Gestion.Infrastructure.data;
using MySql.Data.MySqlClient;

namespace Gestion.core.interfaces.service;

public interface IBaseService<T>
{
    Task<T?> FindById(long id);
    Task<List<T>> FindAll();
    Task<bool> DeleteById(long id);
    Task<bool> Update(T entity);
    Task<bool> Save(T entity);
    Task<List<T>> FindAllByEmpresa(long empresaId);
    Task<List<T>> FindPageByEmpresa(long empresaId, int pageNumber, int pageSize);
    Task<long> ContarPorEmpresa(long empresaId);
}