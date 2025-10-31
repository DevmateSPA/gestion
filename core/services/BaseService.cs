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

    public async Task<T> FindById(int id)
    {
        return await _baseRepository.FindById(id);
    }

    public async Task<List<T>> FindAll()
    {
        return await _baseRepository.FindAll();
    }

    public async Task<bool> DeleteById(int id)
    {
        return await _baseRepository.DeleteById(id);
    }

    public async Task<T> Save(T entity)
    {
        return await _baseRepository.Save(entity);
    }
}