using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Gestion.core.attributes;
using Gestion.presentation.enums;

namespace Gestion.presentation.views.util.buildersUi;

public class FormularioBuilder
{
    private object? _entidad;
    private Panel? _contenedor;
    private Dictionary<PropertyInfo, FrameworkElement>? _controles;

    private int _maxPorFila = 3;
    private ModoFormulario _modo = ModoFormulario.Edicion;

    public FormularioBuilder SetEntidad(object entidad)
    {
        _entidad = entidad;
        return this;
    }

    public FormularioBuilder SetContenedor(Panel contenedor)
    {
        _contenedor = contenedor;
        return this;
    }

    public FormularioBuilder SetControles(Dictionary<PropertyInfo, FrameworkElement> controles)
    {
        _controles = controles;
        return this;
    }

    public FormularioBuilder SetModo(ModoFormulario modo)
    {
        _modo = modo;
        return this;
    }

    public FormularioBuilder SetMaxFila(int max)
    {
        _maxPorFila = max;
        return this;
    }

    public void Build()
    {
        if (_entidad == null)
            throw new InvalidOperationException("Entidad no definida");

        if (_contenedor == null)
            throw new InvalidOperationException("Contenedor no definido");

        if (_controles == null)
            throw new InvalidOperationException("Diccionario de controles no definido");

        _contenedor.Children.Clear();
        _controles.Clear();

        var grupos = ObtenerGruposFormulario(_entidad);

        foreach (var grupo in grupos)
        {
            var groupBox = CrearGroupBox(
                grupo,
                _entidad,
                _controles,
                _maxPorFila);

            _contenedor.Children.Add(groupBox);
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
        Dictionary<PropertyInfo, FrameworkElement> controles,
        ModoFormulario modo)
    {
        var label = LabelFactory.Crear(prop);
        var control = FieldFactory.Crear(prop, entidad, modo);

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
        Dictionary<PropertyInfo, FrameworkElement> controles,
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

            var bloque = CrearBloqueCampo(
                grupo.Propiedades[i],
                entidad,
                controles,
                _modo);

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