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

        return [.. props
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
                Propiedades = [.. g.OrderBy(p => p.GetCustomAttribute<OrdenAttribute>()?.Index ?? int.MaxValue)]
            })
            .OrderBy(g => g.Orden)];
    }

    private static StackPanel CrearBloqueCampo(
        PropertyInfo prop,
        object entidad,
        Dictionary<PropertyInfo, FrameworkElement> controles,
        ModoFormulario modo)
    {
        var label = LabelFactory.Crear(prop);
        var control = FieldFactory.Crear(prop, entidad, modo);
        // Asignamos lo más a la izquierda de su contenedor
        control.HorizontalAlignment = HorizontalAlignment.Left;

        controles[prop] = control;

        var bloque = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0, 0, 0, 10)
        };

        bloque.Children.Add(label);

        // Si es radio button, usamos WrapPanel para que no se corte
        if (EsRadioButton(prop) || EsTextArea(prop))
        {
            var wrap = new WrapPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            wrap.Children.Add(control);
            bloque.Children.Add(wrap);
        }
        else
        {
            bloque.Children.Add(control);
        }

        return bloque;
    }

    private GroupBox CrearGroupBox(
        GrupoFormulario grupo,
        object entidad,
        Dictionary<PropertyInfo, FrameworkElement> controles,
        int maxPorFila)
    {
        var panel = new StackPanel { Margin = new Thickness(10) };

        AgregarPropiedadesAGrupo(panel, grupo.Propiedades, entidad, controles, maxPorFila);

        return new GroupBox
        {
            Header = grupo.Nombre,
            Margin = new Thickness(0, 0, 0, 15),
            Content = panel
        };
    }

    private void AgregarPropiedadesAGrupo(
        StackPanel panel,
        List<PropertyInfo> propiedades,
        object entidad,
        Dictionary<PropertyInfo, FrameworkElement> controles,
        int maxPorFila)
    {
        StackPanel? filaActual = null;
        int columnasEnFila = 0;

        foreach (var prop in propiedades)
        {
            var bloque = CrearBloqueCampo(prop, entidad, controles, _modo);

            if (EsTextArea(prop) || EsRadioButton(prop)) // 1 por fila
            {
                panel.Children.Add(CrearFilaCompleta(bloque));
                filaActual = null;
                columnasEnFila = 0;
            }
            else
            {
                if (filaActual == null || columnasEnFila >= maxPorFila)
                {
                    filaActual = CrearFilaNormal();
                    panel.Children.Add(filaActual);
                    columnasEnFila = 0;
                }

                filaActual.Children.Add(bloque);
                columnasEnFila++;
            }
        }
    }

    private static bool EsTextArea(PropertyInfo prop) =>
        prop.GetCustomAttribute<TextAreaAttribute>() != null;

    private static bool EsRadioButton(PropertyInfo prop) =>
        prop.GetCustomAttribute<RadioGroupAttribute>() != null;

    private static StackPanel CrearFilaCompleta(UIElement elemento) =>
        new()
        {
            Orientation = Orientation.Vertical,
            Children = { elemento },
            Margin = new Thickness(0, 5, 0, 5)
        };

    private static StackPanel CrearFilaNormal() =>
        new()
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 5, 0, 5)
        };
}