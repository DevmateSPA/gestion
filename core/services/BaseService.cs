using Gestion.core.exceptions;
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

    public virtual async Task<bool> Save(T entity)
    {
        List<string> errores = await ValidarReglasNegocio(entity);
        BaseService<T>.AplicarReglasNegocio(errores);

        return await _baseRepository.Save(entity);
    }

    public async Task<List<T>> FindAllByEmpresa(long empresaid)
    {
        return await _baseRepository.FindAllByEmpresa(empresaid);
    }

    public async Task<long> ContarPorEmpresa(long empresaId)
    {
        return await _baseRepository.ContarPorEmpresa(empresaId);
    }
    public virtual async Task<List<T>> FindPageByEmpresa(long empresaId, int pageNumber, int pageSize)
    {
        return await _baseRepository.FindPageByEmpresa(empresaId, pageNumber, pageSize);
    }
    public Task<List<T>> FindAllByParam(String tableOrView,MySqlParameter p,String where)
    {

        return _baseRepository.FindWhereFrom(tableOrView,where, null, null,null,p);
    }

    protected abstract Task<List<string>> ValidarReglasNegocio(T entity);

    private static void AplicarReglasNegocio(List<string> errores)
    {
        if (errores.Count != 0)
            throw new ReglaNegocioException(string.Join("\n", errores));
    }
}