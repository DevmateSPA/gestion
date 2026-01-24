using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        if (_entidad == null)
            throw new InvalidOperationException("Entidad no definida en el builder.");

        if (_contenedor == null)
            throw new InvalidOperationException("Contenedor no definido en el builder.");

        var colecciones = _entidad.GetType()
            .GetProperties()
            .Where(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
            .ToList();

        if (colecciones.Count == 0)
        {
            Debug.WriteLine($"[DataGridBuilder] No se encontraron ObservableCollection en {_entidad.GetType().Name}");
            return;
        }

        foreach (var prop in colecciones)
        {
            Debug.WriteLine($"[DataGridBuilder] Creando tabla para {prop.Name}");
            CrearTabla(prop);
        }
    }

    private void CrearTabla(PropertyInfo prop)
    {
        var itemType = prop.PropertyType.GetGenericArguments().First();

        var groupBox = new GroupBox
        {
            Header = prop.Name,
            Margin = new Thickness(0, 10, 0, 10)
        };

        var grid = new Grid();
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        groupBox.Content = grid;
        _contenedor!.Children.Add(groupBox);

        var dgBuilderType = typeof(DataGridBuilder<>).MakeGenericType(itemType);
        dynamic dgBuilder = Activator.CreateInstance(dgBuilderType)!;

        if (_modo == ModoFormulario.Edicion)
            dgBuilder.Editable();
        else
            dgBuilder.SoloLectura();

        // Columnas
        var columns = DataGridColumnRegistry.Get(itemType);
        if (columns != null)
        {
            for (int i = 0; i < columns.Count; i++)
            {
                var col = columns[i];
                bool fillRemaining = !col.Width.HasValue && i == 0;
                dgBuilder.AddColumna(col.Header, col.Binding, col.Width, fillRemaining);
            }
        }

        dgBuilder
            .SetContenedor(grid)
            .BindItemsSource(_entidad, prop)
            .SetMaxHeight(150)
            .Build();
    }
}