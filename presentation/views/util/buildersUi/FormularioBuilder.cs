using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Gestion.core.attributes;

namespace Gestion.presentation.views.util.buildersUi;

public class FormularioBuilder
{
    public void Build(
        object entidad,
        Panel contenedor,
        Dictionary<PropertyInfo, Control> controles,
        int maxPorFila = 3)
    {
        contenedor.Children.Clear();
        controles.Clear();

        var grupos = ObtenerGruposFormulario(entidad);

        foreach (var grupo in grupos)
        {
            var groupBox = CrearGroupBox(grupo, entidad, controles, maxPorFila);
            contenedor.Children.Add(groupBox);
        }
    }

    private static List<GrupoFormulario> ObtenerGruposFormulario(object entidad)
    {
        var props = FormularioReflectionHelper.ObtenerPropiedadesFormulario(entidad);

        return props
            .GroupBy(p =>
            {
                var grupo = p.GetCustomAttribute<GrupoAttribute>();
                return grupo == null
                    ? ("General", int.MaxValue)
                    : (grupo.Nombre, grupo.Index);
            })
            .Select(g => new GrupoFormulario
            {
                Nombre = g.Key.Item1,
                Orden = g.Key.Item2,
                Propiedades = g
                    .OrderBy(p => p.GetCustomAttribute<OrdenAttribute>()?.Index ?? int.MaxValue)
                    .ToList()
            })
            .OrderBy(g => g.Orden)
            .ToList();
    }

    private StackPanel CrearFila() => new() { Orientation = Orientation.Horizontal };

    private StackPanel CrearBloqueCampo(
        PropertyInfo prop,
        object entidad,
        Dictionary<PropertyInfo, Control> controles)
    {
        var label = LabelFactory.Crear(prop);
        var control = FieldFactory.Crear(prop, entidad);

        controles[prop] = control;

        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Width = 310,
            Children = { label, control }
        };
    }

    private GroupBox CrearGroupBox(
        GrupoFormulario grupo,
        object entidad,
        Dictionary<PropertyInfo, Control> controles,
        int maxPorFila)
    {
        var panel = new StackPanel { Margin = new Thickness(10) };

        StackPanel? filaActual = null;

        for (int i = 0; i < grupo.Propiedades.Count; i++)
        {
            if (i % maxPorFila == 0)
            {
                filaActual = CrearFila();
                panel.Children.Add(filaActual);
            }

            var bloque = CrearBloqueCampo(grupo.Propiedades[i], entidad, controles);
            filaActual!.Children.Add(bloque);
        }

        return new GroupBox
        {
            Header = grupo.Nombre,
            Margin = new Thickness(0, 0, 0, 15),
            Content = panel
        };
    }
}