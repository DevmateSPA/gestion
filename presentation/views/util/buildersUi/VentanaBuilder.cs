using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Gestion.presentation.enums;

namespace Gestion.presentation.views.util.buildersUi;

public class VentanaBuilder<TEntidad>
{
    private readonly FormularioBuilder _fomularioBuilder = new();
    private Dictionary<PropertyInfo, FrameworkElement> _controles = [];
    private Panel? _contenedorCampos;
    private Panel? _contenedorTablas;
    private TEntidad? _entidad;
    private ModoFormulario _modo = ModoFormulario.Edicion;

    public VentanaBuilder<TEntidad> SetEntidad(TEntidad entidad)
    {
        _entidad = entidad;
        return this;
    }

    public VentanaBuilder<TEntidad> SetContenedorCampos(Panel contenedor)
    {
        _contenedorCampos = contenedor;
        return this;
    }

    public VentanaBuilder<TEntidad> SetContenedorTablas(Panel contenedor)
    {
        _contenedorTablas = contenedor;
        return this;
    }

    public VentanaBuilder<TEntidad> SetModo(ModoFormulario modo)
    {
        _modo = modo;
        return this;
    }

    public void Build()
    {
        // Llamamos el builder de UI
        _fomularioBuilder
            .SetEntidad(_entidad!)
            .SetContenedor(_contenedorCampos!)
            .SetControles(_controles)
            .SetModo(_modo)
            .Build();

        // Colección ("Detalles") que tenga la entidad?
        // Genera un dg

        var colecciones = _entidad!
            .GetType()
            .GetProperties()
            .Where(p => typeof(IEnumerable).IsAssignableFrom(p.PropertyType) 
                && p.PropertyType != typeof(string));

        foreach (var prop in colecciones)
        {
            var items = (prop.GetValue(_entidad) as IEnumerable)?.Cast<object>().ToList();
            if (items == null || items.Count == 0) continue;

            var dgBuilderType = typeof(DataGridBuilder<>)
                .MakeGenericType(prop.PropertyType.GetGenericArguments().FirstOrDefault() ?? typeof(object));

            dynamic dgBuilder = Activator.CreateInstance(dgBuilderType)!;

            // Editable según el modo del formulario
            if (_modo == ModoFormulario.Edicion)
                dgBuilder.Editable();
            else
                dgBuilder.SoloLectura();

            dgBuilder
                .SetContenedor(_contenedorTablas)
                .SetItems(items)
                .Build();
        }
    }

    public Dictionary<PropertyInfo, FrameworkElement> GetControles() => _controles;
}