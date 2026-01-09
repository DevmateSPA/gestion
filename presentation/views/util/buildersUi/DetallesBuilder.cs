using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Gestion.presentation.enums;

namespace Gestion.presentation.views.util.buildersUi;

/// <summary>
/// Builder encargado de generar automáticamente tablas de detalle
/// (DataGrids) para propiedades de tipo colección dentro de una entidad.
/// </summary>
/// <remarks>
/// Este builder inspecciona la entidad mediante reflexión y crea
/// una tabla por cada propiedad que sea:
/// <list type="bullet">
/// <item>Implementación de <see cref="IEnumerable"/></item>
/// <item>Genérica</item>
/// <item>Distinta de <see cref="string"/></item>
/// </list>
///
/// Cada tabla se renderiza dentro de un <see cref="GroupBox"/>,
/// permitiendo mostrar relaciones uno-a-muchos en formularios
/// de edición o solo lectura.
/// </remarks>
public class DetallesBuilder
{
    private object? _entidad;
    private Panel? _contenedor;
    private ModoFormulario _modo;

    /// <summary>
    /// Define la entidad desde la cual se obtendrán las colecciones
    /// a mostrar como tablas de detalle.
    /// </summary>
    public DetallesBuilder SetEntidad(object entidad)
    {
        _entidad = entidad;
        return this;
    }

    /// <summary>
    /// Define el contenedor visual donde se agregarán las tablas
    /// generadas dinámicamente.
    /// </summary>
    public DetallesBuilder SetContenedor(Panel contenedor)
    {
        _contenedor = contenedor;
        return this;
    }

    /// <summary>
    /// Define el modo de visualización del detalle
    /// (edición o solo lectura).
    /// </summary>
    public DetallesBuilder SetModo(ModoFormulario modo)
    {
        _modo = modo;
        return this;
    }

    /// <summary>
    /// Construye todas las tablas de detalle para las colecciones
    /// encontradas en la entidad.
    /// </summary>
    /// <remarks>
    /// Si la entidad o el contenedor no han sido definidos,
    /// el método termina sin ejecutar ninguna acción.
    /// </remarks>
    public void Build()
    {
        if (_entidad == null || _contenedor == null)
            return;

        var colecciones = _entidad.GetType()
            .GetProperties()
            .Where(p =>
                typeof(IEnumerable).IsAssignableFrom(p.PropertyType) &&
                p.PropertyType != typeof(string) &&
                p.PropertyType.IsGenericType);

        foreach (var prop in colecciones)
            CrearTabla(prop);
    }

    /// <summary>
    /// Crea una tabla de detalle para una propiedad de tipo colección.
    /// </summary>
    /// <param name="prop">
    /// Propiedad de la entidad que representa una colección genérica.
    /// </param>
    /// <remarks>
    /// El DataGrid se construye dinámicamente usando:
    /// <list type="bullet">
    /// <item><see cref="DataGridBuilder{T}"/> para el tipo de ítem</item>
    /// <item><see cref="DataGridColumnRegistry"/> para definir columnas por tipo</item>
    /// </list>
    ///
    /// El comportamiento editable o de solo lectura depende del
    /// <see cref="ModoFormulario"/> configurado.
    /// </remarks>
    private void CrearTabla(PropertyInfo prop)
    {
        var itemType = prop.PropertyType.GetGenericArguments().First();
        var items = (IEnumerable)prop.GetValue(_entidad)!;

        var groupBox = new GroupBox
        {
            Header = prop.Name,
            Margin = new Thickness(0, 10, 0, 10)
        };

        var grid = new Grid();
        grid.RowDefinitions.Add(
            new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

        groupBox.Content = grid;
        _contenedor!.Children.Add(groupBox);

        var dgBuilderType = typeof(DataGridBuilder<>).MakeGenericType(itemType);
        dynamic dgBuilder = Activator.CreateInstance(dgBuilderType)!;

        if (_modo == ModoFormulario.Edicion)
            dgBuilder.Editable();
        else
            dgBuilder.SoloLectura();

        // Configuración de columnas registrada por tipo
        var columns = DataGridColumnRegistry.Get(itemType);
        if (columns != null)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                var col = columns[i];
                // Por defecto, la primera columna de texto ocupa el ancho restante
                bool fillRemaining = !col.Width.HasValue && i == 0;
                dgBuilder.AddColumna(col.Header, col.Binding, col.Width, fillRemaining);
            }
        }

        dgBuilder
            .SetContenedor(grid)
            .SetItems(items)
            .SetMaxHeight(150)
            .Build();
    }
}