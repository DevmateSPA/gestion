using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Gestion.presentation.enums;

namespace Gestion.presentation.views.util.buildersUi;

public class DetallesBuilder
{
    private object? _entidad;
    private Panel? _contenedor;
    private ModoFormulario _modo;

    public DetallesBuilder SetEntidad(object entidad)
    {
        _entidad = entidad;
        return this;
    }

    public DetallesBuilder SetContenedor(Panel contenedor)
    {
        _contenedor = contenedor;
        return this;
    }

    public DetallesBuilder SetModo(ModoFormulario modo)
    {
        _modo = modo;
        return this;
    }

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

        // ⭐ CONFIGURACIÓN POR TIPO (AQUÍ ES DONDE VA)
        var columns = DataGridColumnRegistry.Get(itemType);
        if (columns != null)
        {
            foreach (var col in columns)
                dgBuilder.AddColumna(col.Header, col.Binding, col.Width);
        }

        dgBuilder
            .SetContenedor(grid)
            .SetItems(items)
            .SetMaxHeight(150)
            .Build();
    }
}