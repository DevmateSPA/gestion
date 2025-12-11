using Gestion.core.interfaces.model;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;
using MySql.Data.MySqlClient;

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

    public Task<List<T>> FindAllByEmpresa(long empresaid)
    {
        return _baseRepository.FindAllByEmpresa(empresaid);
    }
    public Task<List<T>> FindAllByParam(String tableOrView,MySqlParameter p,String where)
    {

        return _baseRepository.FindWhereFrom(tableOrView,where,p);
    }
}