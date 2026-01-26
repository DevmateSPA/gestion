using Gestion.core.exceptions;
using Gestion.core.interfaces.model;
using Gestion.core.interfaces.reglas;
using Gestion.core.interfaces.repository;
using Gestion.core.interfaces.service;

namespace Gestion.core.services;

/// <summary>
/// Servicio base genérico para entidades del dominio.
///
/// Actúa como capa intermedia entre los repositorios y la capa de presentación,
/// centralizando:
/// - Acceso a datos
/// - Validación de reglas de negocio
/// - Flujo consistente de operaciones CRUD
///
/// Está pensado para ser heredado por servicios concretos
/// (Ej: ClienteService, FacturaService, ProductoService, etc.).
/// </summary>
/// <typeparam name="T">
/// Tipo de entidad manejada por el servicio.
/// Debe implementar <see cref="IModel"/>.
/// </typeparam>
public abstract class BaseService<T> : IBaseService<T>
    where T : IModel
{
    /// <summary>
    /// Repositorio base asociado a la entidad.
    /// </summary>
    private readonly IBaseRepository<T> _baseRepository;

    /// <summary>
    /// Inicializa el servicio base con su repositorio correspondiente.
    /// </summary>
    /// <param name="baseRepository">
    /// Repositorio que maneja el acceso a datos de la entidad.
    /// </param>
    protected BaseService(IBaseRepository<T> baseRepository)
    {
        _baseRepository = baseRepository;
    }

    #region Métodos de consulta

    /// <summary>
    /// Obtiene una entidad por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <returns>
    /// La entidad encontrada o <c>null</c> si no existe.
    /// </returns>
    public async Task<T?> FindById(long id)
    {
        return await _baseRepository.FindById(id);
    }

    /// <summary>
    /// Obtiene todas las entidades disponibles.
    /// </summary>
    public async Task<List<T>> FindAll()
    {
        return await _baseRepository.FindAll();
    }

    /// <summary>
    /// Obtiene todas las entidades asociadas a una empresa.
    /// </summary>
    /// <param name="empresaid">Identificador de la empresa.</param>
    public async Task<List<T>> FindAllByEmpresa(long empresaid)
    {
        return await _baseRepository.FindAllByEmpresa(empresaid);
    }

    /// <summary>
    /// Obtiene el total de registros asociados a una empresa.
    /// </summary>
    /// <param name="empresaId">Identificador de la empresa.</param>
    public async Task<long> ContarPorEmpresa(long empresaId)
    {
        return await _baseRepository.ContarPorEmpresa(empresaId);
    }

    /// <summary>
    /// Obtiene una página de resultados asociados a una empresa.
    /// </summary>
    /// <param name="empresaId">Identificador de la empresa.</param>
    /// <param name="pageNumber">Número de página (1-based).</param>
    /// <param name="pageSize">Cantidad de registros por página.</param>
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


    #endregion

    #region Operaciones CRUD

    /// <summary>
    /// Elimina una entidad por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la entidad.</param>
    /// <returns>
    /// <c>true</c> si la eliminación fue exitosa.
    /// </returns>
    public async Task<bool> DeleteById(long id)
    {
        return await _baseRepository.DeleteById(id);
    }

    /// <summary>
    /// Actualiza una entidad existente.
    ///
    /// Antes de persistir los cambios, se validan las reglas de negocio.
    /// </summary>
    /// <param name="entity">Entidad a actualizar.</param>
    /// <exception cref="ReglaNegocioException">
    /// Se lanza si alguna regla de negocio no se cumple.
    /// </exception>
    public async Task<bool> Update(T entity)
    {
        await ValidarYAplicarReglas(entity, entity.Id);      

        return await _baseRepository.Update(entity);
    }

    /// <summary>
    /// Guarda una nueva entidad.
    ///
    /// Aplica validaciones de reglas de negocio antes de persistir.
    /// </summary>
    /// <param name="entity">Entidad a guardar.</param>
    /// <exception cref="ReglaNegocioException">
    /// Se lanza si alguna regla de negocio no se cumple.
    /// </exception>
    public virtual async Task<bool> Save(T entity)
    {
        await ValidarYAplicarReglas(entity, null);

        return await _baseRepository.Save(entity);
    }

    #endregion

    #region Reglas de negocio

    /// <summary>
    /// Valida las reglas de negocio específicas de la entidad.
    ///
    /// Debe ser implementado por los servicios concretos.
    /// </summary>
    /// <param name="entity">Entidad a validar.</param>
    /// <param name="excludeId">
    /// Identificador a excluir en validaciones de unicidad
    /// (usado normalmente en actualizaciones).
    /// </param>
    /// <returns>
    /// Lista de mensajes de error. Si está vacía, la entidad es válida.
    /// </returns>
    protected abstract IEnumerable<IReglaNegocio<T>> DefinirReglas(
        T entity,
        long? excludeId);

    /// <summary>
    /// Aplica el resultado de las validaciones de negocio.
    ///
    /// Si existen errores, lanza una excepción de dominio.
    /// </summary>
    /// <param name="errores">Lista de errores de validación.</param>
    /// <exception cref="ReglaNegocioException">
    /// Se lanza cuando existen reglas incumplidas.
    /// </exception>
    private static void AplicarReglasNegocio(List<string> errores)
    {
        if (errores.Count != 0)
            throw new ReglaNegocioException(
                string.Join("\n", errores));
    }

    private async Task ValidarYAplicarReglas(
        T entity,
        long? excludeId)
    {
        var reglas = DefinirReglas(entity, excludeId);

        List<string> errores = [];

        foreach (var regla in reglas)
        {
            var error = await regla.Validar(entity, excludeId);
            if (error != null)
                errores.Add(error);
        }

        AplicarReglasNegocio(errores);
    }

    #endregion
}
