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
            // Obtiene el Tipo
            var itemType = prop.PropertyType.GetGenericArguments().First();

            // Transforma la colección
            IEnumerable enumerableTyped = (IEnumerable) typeof(Enumerable)
                .GetMethod(nameof(Enumerable.Cast))!
                .MakeGenericMethod(itemType)
                .Invoke(null, [prop.GetValue(_entidad)!])!;

            // Crea un Group Box
            var groupBox = new GroupBox
            {
                Header = $"Detalles",    // Título del grupo
                Margin = new Thickness(0, 10, 0, 10)
            };

            // Crear contenedor interno del DataGrid
            var stackPanel = new StackPanel();
            groupBox.Content = stackPanel;

            // Agregar el GroupBox al contenedor principal de tablas
            _contenedorTablas!.Children.Add(groupBox);

            // Crear el DataGridBuilder<T>
            var dgBuilderType = typeof(DataGridBuilder<>).MakeGenericType(itemType);
            dynamic dgBuilder = Activator.CreateInstance(dgBuilderType)!;

            // Modo edición / lectura
            if (_modo == ModoFormulario.Edicion)
                dgBuilder.Editable();
            else
                dgBuilder.SoloLectura();

            // Construir el DataGrid DENTRO del GroupBox
            dgBuilder
                .SetContenedor(stackPanel)   // aquí va dentro del GroupBox
                .SetItems(enumerableTyped)
                .Build();
        }
    }

    public Dictionary<PropertyInfo, FrameworkElement> GetControles() => _controles;
}