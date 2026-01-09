using System.Reflection;
using Gestion.core.attributes;

namespace Gestion.presentation.views.util.buildersUi;

/// <summary>
/// Utilidad de reflexión encargada de obtener y filtrar las propiedades
/// de una entidad que deben ser renderizadas en un formulario dinámico.
/// </summary>
/// <remarks>
/// Este helper aplica reglas comunes de construcción de formularios:
/// <list type="bullet">
/// <item>Solo incluye propiedades públicas de instancia con setter.</item>
/// <item>Excluye la propiedad <c>Id</c> por convención.</item>
/// <item>Permite únicamente tipos simples (string y value types).</item>
/// <item>Respeta el atributo <see cref="VisibleAttribute"/>.</item>
/// <item>Ordena por grupo, orden de campo y nombre.</item>
/// </list>
/// </remarks>
public static class FormularioReflectionHelper
{
    /// <summary>
    /// Obtiene las propiedades de una entidad que deben mostrarse
    /// en un formulario dinámico.
    /// </summary>
    /// <param name="entidad">Entidad de dominio a inspeccionar mediante reflexión.</param>
    /// <returns>
    /// Lista ordenada de <see cref="PropertyInfo"/> que cumplen las reglas
    /// de visualización y ordenamiento del formulario.
    /// </returns>
    public static List<PropertyInfo> ObtenerPropiedadesFormulario(object entidad)
    {
        return [
            .. entidad.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p =>
                    // Debe poder escribirse (binding TwoWay)
                    p.CanWrite &&

                    // Se excluye el Id por convención
                    !string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) &&

                    // Solo tipos simples
                    (p.PropertyType == typeof(string) || p.PropertyType.IsValueType) &&

                    // Respeta visibilidad definida por atributo
                    (p.GetCustomAttribute<VisibleAttribute>()?.Mostrar ?? true))
                .Select(p =>
                {
                    var grupo = p.GetCustomAttribute<GrupoAttribute>();
                    var orden = p.GetCustomAttribute<OrdenAttribute>();

                    return new
                    {
                        Prop = p,
                        GrupoOrden = grupo?.Index ?? int.MaxValue,
                        CampoOrden = orden?.Index ?? int.MaxValue,
                        GrupoNombre = grupo?.Nombre
                    };
                })
                // Orden lógico del formulario
                .OrderBy(x => x.GrupoOrden)
                .ThenBy(x => x.CampoOrden)
                .ThenBy(x => x.Prop.Name)
                .Select(x => x.Prop)
        ];
    }
}