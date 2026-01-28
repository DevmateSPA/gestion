using Gestion.core.exceptions;
using Gestion.core.interfaces.reglas;

namespace Gestion.core.reglas.common;

/// <summary>
/// Regla de negocio que valida la unicidad de un valor dentro del dominio.
/// </summary>
/// <typeparam name="T">
/// Tipo de entidad sobre la cual se aplica la regla.
/// </typeparam>
/// <remarks>
/// Esta regla delega la verificación de existencia a una función externa,
/// permitiendo validar unicidad contra cualquier fuente de datos
/// (por ejemplo, base de datos o servicios).
/// 
/// Es especialmente útil en escenarios de creación y actualización,
/// soportando la exclusión de un identificador mediante <c>excludeId</c>.
/// </remarks>
public class UnicoRegla<T> : IReglaNegocio<T>
{
    private readonly Func<T, long?, Task<bool>> _existe;
    private readonly Func<T, object?> _valor;
    private readonly string _mensaje;
    private readonly string? _campo;

    /// <summary>
    /// Inicializa una nueva instancia de la regla de unicidad.
    /// </summary>
    /// <param name="existe">
    /// Función que determina si ya existe una entidad
    /// con el mismo valor dentro del dominio.
    /// 
    /// Debe retornar <c>true</c> cuando el valor ya existe
    /// y <c>false</c> en caso contrario.
    /// </param>
    /// <param name="valor">
    /// Función que obtiene el valor a validar desde la entidad.
    /// Se utiliza únicamente para construir el mensaje de error.
    /// </param>
    /// <param name="mensaje">
    /// Mensaje de error a retornar cuando la regla no se cumple.
    /// Puede incluir placeholders (por ejemplo <c>{0}</c>)
    /// que serán reemplazados por el valor evaluado.
    /// </param>
    /// <param name="campo">
    /// Nombre opcional del campo asociado al error,
    /// utilizado para validaciones a nivel de UI o API.
    /// </param>
    public UnicoRegla(
        Func<T, long?, Task<bool>> existe,
        Func<T, object?> valor,
        string mensaje,
        string? campo = null)
    {
        _existe = existe;
        _valor = valor;
        _mensaje = mensaje;
        _campo = campo;
    }

    /// <summary>
    /// Valida que el valor evaluado sea único dentro del dominio.
    /// </summary>
    /// <param name="entidad">
    /// Entidad a validar.
    /// </param>
    /// <param name="excludeId">
    /// Identificador opcional a excluir de la validación,
    /// usado comúnmente en operaciones de actualización.
    /// </param>
    /// <returns>
    /// Un <see cref="ErrorNegocio"/> si el valor ya existe;
    /// <c>null</c> si la validación es exitosa.
    /// </returns>
    public async Task<ErrorNegocio?> Validar(
        T entidad,
        long? excludeId = null)
    {
        if (!await _existe(entidad, excludeId))
            return null;

        var valor = _valor(entidad);

        return new ErrorNegocio(
            codigo: "UNIQUE",
            mensaje: string.Format(_mensaje, valor),
            campo: _campo);
    }
}