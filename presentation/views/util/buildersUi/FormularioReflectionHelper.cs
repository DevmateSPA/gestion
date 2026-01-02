using System.Reflection;
using Gestion.core.attributes;

namespace Gestion.presentation.views.util.buildersUi;

public static class FormularioReflectionHelper
{
    public static List<PropertyInfo> ObtenerPropiedadesFormulario(object entidad)
    {
        return [
                .. entidad.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p =>
                    p.CanWrite &&
                    !string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) &&
                    (p.PropertyType == typeof(string) || p.PropertyType.IsValueType) &&
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
                .OrderBy(x => x.GrupoOrden)
                .ThenBy(x => x.CampoOrden)
                .ThenBy(x => x.Prop.Name)
                .Select(x => x.Prop)
            ];
    }
}
