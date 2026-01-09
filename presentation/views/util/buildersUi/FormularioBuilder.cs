using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Gestion.core.attributes;
using Gestion.presentation.enums;

namespace Gestion.presentation.views.util.buildersUi;

/// <summary>
/// Builder encargado de generar dinámicamente formularios WPF
/// a partir de una entidad utilizando reflexión y atributos personalizados.
/// </summary>
/// <remarks>
/// Este builder:
/// <list type="bullet">
/// <item>Agrupa propiedades usando <see cref="GrupoAttribute"/>.</item>
/// <item>Ordena campos mediante <see cref="OrdenAttribute"/>.</item>
/// <item>Genera automáticamente controles según el tipo de dato y atributos.</item>
/// <item>Soporta distintos modos de formulario (edición / solo lectura).</item>
/// </list>
/// </remarks>
public class FormularioBuilder
{
    private object? _entidad;
    private Panel? _contenedor;
    private Dictionary<PropertyInfo, FrameworkElement>? _controles;

    private int _maxPorFila = 3;
    private ModoFormulario _modo = ModoFormulario.Edicion;

    /// <summary>
    /// Define la entidad desde la cual se generará el formulario.
    /// </summary>
    /// <param name="entidad">Objeto de dominio a inspeccionar mediante reflexión.</param>
    /// <returns>Instancia del builder para encadenamiento.</returns>
    public FormularioBuilder SetEntidad(object entidad)
    {
        _entidad = entidad;
        return this;
    }

    /// <summary>
    /// Define el contenedor visual donde se renderizará el formulario.
    /// </summary>
    /// <param name="contenedor">Panel WPF que alojará los controles.</param>
    /// <returns>Instancia del builder para encadenamiento.</returns>
    public FormularioBuilder SetContenedor(Panel contenedor)
    {
        _contenedor = contenedor;
        return this;
    }

    /// <summary>
    /// Define el diccionario donde se almacenará la relación
    /// entre propiedades y controles creados.
    /// </summary>
    /// <param name="controles">Diccionario PropertyInfo → FrameworkElement.</param>
    /// <returns>Instancia del builder para encadenamiento.</returns>
    public FormularioBuilder SetControles(Dictionary<PropertyInfo, FrameworkElement> controles)
    {
        _controles = controles;
        return this;
    }

    /// <summary>
    /// Define el modo de funcionamiento del formulario.
    /// </summary>
    /// <param name="modo">Modo del formulario (edición o solo lectura).</param>
    /// <returns>Instancia del builder para encadenamiento.</returns>
    public FormularioBuilder SetModo(ModoFormulario modo)
    {
        _modo = modo;
        return this;
    }

    /// <summary>
    /// Define el número máximo de campos por fila.
    /// </summary>
    /// <param name="max">Cantidad máxima de columnas por fila.</param>
    /// <returns>Instancia del builder para encadenamiento.</returns>
    public FormularioBuilder SetMaxFila(int max)
    {
        _maxPorFila = max;
        return this;
    }

    /// <summary>
    /// Construye el formulario dinámicamente y lo renderiza
    /// dentro del contenedor configurado.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si la entidad, contenedor o diccionario de controles no están definidos.
    /// </exception>
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

    /// <summary>
    /// Obtiene y agrupa las propiedades de la entidad según los atributos definidos.
    /// </summary>
    /// <param name="entidad">Entidad de dominio.</param>
    /// <returns>Lista ordenada de grupos de formulario.</returns>
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

    /// <summary>
    /// Crea el bloque visual de un campo individual (label + control).
    /// </summary>
    private static StackPanel CrearBloqueCampo(
        PropertyInfo prop,
        object entidad,
        Dictionary<PropertyInfo, FrameworkElement> controles,
        ModoFormulario modo)
    {
        var label = LabelFactory.Crear(prop);
        var control = FieldFactory.Crear(prop, entidad, modo);

        control.HorizontalAlignment = HorizontalAlignment.Left;
        controles[prop] = control;

        var bloque = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(0, 0, 0, 10)
        };

        bloque.Children.Add(label);

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

    /// <summary>
    /// Crea un <see cref="GroupBox"/> que contiene un grupo de campos del formulario.
    /// </summary>
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

    /// <summary>
    /// Agrega las propiedades al panel respetando el layout por filas.
    /// </summary>
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

            if (EsTextArea(prop) || EsRadioButton(prop))
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

    /// <summary>
    /// Indica si la propiedad representa un campo de texto multilínea.
    /// </summary>
    private static bool EsTextArea(PropertyInfo prop) =>
        prop.GetCustomAttribute<TextAreaAttribute>() != null;

    /// <summary>
    /// Indica si la propiedad se representa mediante un grupo de RadioButtons.
    /// </summary>
    private static bool EsRadioButton(PropertyInfo prop) =>
        prop.GetCustomAttribute<RadioGroupAttribute>() != null;

    /// <summary>
    /// Crea una fila que ocupa todo el ancho disponible.
    /// </summary>
    private static StackPanel CrearFilaCompleta(UIElement elemento) =>
        new()
        {
            Orientation = Orientation.Vertical,
            Children = { elemento },
            Margin = new Thickness(0, 5, 0, 5)
        };

    /// <summary>
    /// Crea una fila normal con disposición horizontal.
    /// </summary>
    private static StackPanel CrearFilaNormal() =>
        new()
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 5, 0, 5)
        };
}