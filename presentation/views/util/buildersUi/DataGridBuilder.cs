using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Gestion.presentation.views.util.buildersUi;

public class DataGridBuilder<T>
{
    private readonly DataGrid _dg = new();
    private Panel? _contenedor;
    private bool _tieneColumnas = false;

    public DataGridBuilder<T> SoloLectura()
    {
        _dg.IsReadOnly = true;
        _dg.CanUserAddRows = false;
        _dg.CanUserDeleteRows = false;
        return this;
    }

    public DataGridBuilder<T> Editable()
    {
        _dg.IsReadOnly = false;
        _dg.CanUserAddRows = true;
        _dg.CanUserDeleteRows = true;
        return this;
    }

    public DataGridBuilder<T> SetContenedor(Panel contenedor)
    {
        _contenedor = contenedor;
        return this;
    }

    public DataGridBuilder<T> SetItems(IEnumerable items)
    {
        _dg.ItemsSource = items;
        return this;
    }

    public DataGridBuilder<T> SetMaxHeight(double maxHeight)
    {
        _dg.MaxHeight = maxHeight;
        return this;
    }

    public DataGridBuilder<T> AddColumna(
        string header,
        string bindingPath,
        double? width = null,
        bool fillRemaining = false)
    {
        _tieneColumnas = true;

        var col = new DataGridTextColumn
        {
            Header = header,
            Binding = new Binding(bindingPath),
            Width = width.HasValue
                ? new DataGridLength(width.Value)
                : fillRemaining
                    ? new DataGridLength(1, DataGridLengthUnitType.Star)
                    : new DataGridLength(1, DataGridLengthUnitType.Auto),
            MinWidth = 10
        };

        _dg.Columns.Add(col);

        if (_dg.ColumnHeaderStyle == null)
        {
            _dg.ColumnHeaderStyle = new Style(typeof(DataGridColumnHeader))
            {
                Setters =
                {
                    new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap),
                    new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Center)
                }
            };
        }

        return this;
    }

    public void Build()
    {
        if (_contenedor == null)
            throw new InvalidOperationException("Contenedor no definido");

        if (_contenedor is StackPanel)
            throw new InvalidOperationException(
                "DataGrid no puede alojarse en StackPanel. Use Grid o DockPanel.");

        _dg.AutoGenerateColumns = !_tieneColumnas;

        _dg.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        _dg.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

        if (double.IsNaN(_dg.MaxHeight) || _dg.MaxHeight == 0)
            _dg.MaxHeight = 250;

        _contenedor.Children.Add(_dg);
    }
}