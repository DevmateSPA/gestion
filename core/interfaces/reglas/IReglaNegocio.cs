using Gestion.core.exceptions;

namespace Gestion.core.interfaces.reglas;

/// <summary>
/// Define una regla de negocio aplicable a una entidad del dominio.
/// </summary>
/// <typeparam name="T">
/// Tipo de entidad sobre la cual se aplica la regla.
/// </typeparam>
/// <remarks>
/// Una regla de negocio encapsula una validación específica
/// del dominio y puede depender de recursos externos
/// (por ejemplo, consultas a base de datos).
/// 
/// Si la regla no se cumple, retorna un <see cref="ErrorNegocio"/>;
/// en caso contrario, retorna <c>null</c>.
/// </remarks>
public interface IReglaNegocio<T>
{
        /// <summary>
    /// Valida la entidad según la regla de negocio implementada.
    /// </summary>
    /// <param name="entidad">
    /// Entidad a validar.
    /// </param>
    /// <param name="excludeId">
    /// Identificador opcional a excluir durante la validación,
    /// utilizado principalmente en operaciones de actualización.
    /// </param>
    /// <returns>
    /// Un <see cref="ErrorNegocio"/> si la regla no se cumple;
    /// <c>null</c> si la validación es exitosa.
    /// </returns>
    Task<ErrorNegocio?> Validar(
        T entidad, 
        long? excludeId = null);
}