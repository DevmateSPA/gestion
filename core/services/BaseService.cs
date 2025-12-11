using Gestion.core.interfaces.model;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;

namespace Gestion.core.services;

public abstract class BaseService<T> : IBaseService<T> where T : IModel
{
    private readonly IBaseRepository<T> _baseRepository;

    public BaseService(IBaseRepository<T> baseRepository)
    {
        _baseRepository = baseRepository;
    }

    public async Task<T?> FindById(long id)
    {
        return await _baseRepository.FindById(id);
    }

    public async Task<List<T>> FindAll()
    {
        return await _baseRepository.FindAll();
    }

    public async Task<bool> DeleteById(long id)
    {
        return await _baseRepository.DeleteById(id);
    }

    public async Task<bool> Update(T entity)
    {
        return await _baseRepository.Update(entity);
    }

    public async Task<bool> Save(T entity)
    {
        return await _baseRepository.Save(entity);
    }

    public async Task<List<T>> FindAllByEmpresa(long empresaid)
    {
        return await _baseRepository.FindAllByEmpresa(empresaid);
    }

    public async Task<long> ContarPorEmpresa(long empresaId)
    {
        var where = "empresa = @empresa";
        var p = new MySql.Data.MySqlClient.MySqlParameter("@empresa", empresaId);
        return await _baseRepository.CountWhere(where, p);
    }
    public virtual Task<List<T>> FindPageByEmpresa(long empresaId, int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }
}