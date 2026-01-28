using Gestion.core.exceptions;
using Gestion.core.interfaces.model;
using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;

namespace Gestion.core.services;

public abstract class BaseService<T> : IBaseService<T>
    where T : IModel, new()
{
    private readonly IBaseRepository<T> _baseRepository;

    protected BaseService(IBaseRepository<T> baseRepository)
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

    public async Task<List<T>> FindAllByEmpresa(long empresaid)
    {
        return await _baseRepository.FindAllByEmpresa(empresaid);
    }

    public async Task<long> ContarPorEmpresa(long empresaId)
    {
        return await _baseRepository.ContarPorEmpresa(empresaId);
    }

    public virtual async Task<List<T>> FindPageByEmpresa(
        long empresaId,
        int pageNumber,
        int pageSize)
    {
        return await _baseRepository.FindPageByEmpresa(
            empresaId,
            pageNumber,
            pageSize);
    }

    public async Task<bool> DeleteById(long id)
    {
        return await _baseRepository.DeleteById(id);
    }

    public async Task<bool> Update(T entity)
    {
        await ValidarYAplicarReglas(entity, entity.Id);      

        return await _baseRepository.Update(entity);
    }

    public virtual async Task<bool> Save(T entity)
    {
        await ValidarYAplicarReglas(entity, null);

        return await _baseRepository.Save(entity);
    }

    /// <summary>
    /// Define el conjunto de reglas de negocio que deben validarse
    /// para una entidad antes de persistirla.
    /// </summary>
    /// <param name="entity">
    /// Entidad a validar.
    /// </param>
    /// <param name="excludeId">
    /// Identificador opcional a excluir durante la validación,
    /// utilizado principalmente en operaciones de actualización.
    /// </param>
    /// <remarks>
    /// Cada implementación concreta del servicio debe retornar
    /// las reglas de negocio aplicables al contexto de la entidad.
    /// 
    /// Las reglas se evalúan de forma asíncrona y centralizada
    /// mediante el mecanismo de validación del servicio base.
    /// </remarks>
    /// <returns>
    /// Colección de reglas de negocio a evaluar.
    /// </returns>
    protected abstract IEnumerable<IReglaNegocio<T>> DefinirReglas(
        T entity,
        long? excludeId);

    /// <summary>
    /// Aplica el resultado de la validación de reglas de negocio,
    /// lanzando una excepción si existen errores.
    /// </summary>
    /// <param name="errores">
    /// Lista de errores de negocio detectados durante la validación.
    /// </param>
    /// <remarks>
    /// Este método centraliza la decisión de corte ante errores
    /// de negocio, evitando que las capas superiores deban
    /// interpretar resultados parciales.
    /// </remarks>
    private static void AplicarReglasNegocio(
        List<ErrorNegocio> errores)
    {
        if (errores.Count != 0)
            throw new ReglaNegocioException(errores);
    }

    /// <summary>
    /// Valida una entidad contra sus reglas de negocio y
    /// aplica el resultado de la validación.
    /// </summary>
    /// <param name="entity">
    /// Entidad a validar.
    /// </param>
    /// <param name="excludeId">
    /// Identificador opcional a excluir durante la validación.
    /// </param>
    /// <remarks>
    /// El flujo de validación es:
    /// 
    /// 1. Se obtienen las reglas mediante <see cref="DefinirReglas"/>
    /// 2. Se evalúan todas las reglas de forma asíncrona
    /// 3. Se recopilan los errores de negocio
    /// 4. Se aplica el resultado mediante <see cref="AplicarReglasNegocio"/>
    /// 
    /// Si existe al menos un error, se lanza una
    /// <see cref="ReglaNegocioException"/>.
    /// </remarks>
    private async Task ValidarYAplicarReglas(
        T entity,
        long? excludeId)
    {
        var reglas = DefinirReglas(entity, excludeId);

        List<ErrorNegocio> errores = [];

        foreach (var regla in reglas)
        {
            var error = await regla.Validar(entity, excludeId);
            if (error is not null)
                errores.Add(error);
        }

        AplicarReglasNegocio(errores);
    }
}
