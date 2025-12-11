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

    Task<List<T>> FindAllByParam(String tableOrView,MySqlParameter p, String where);
}