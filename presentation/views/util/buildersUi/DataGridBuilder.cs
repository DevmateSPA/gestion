using System.Windows.Controls;
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

    public DataGridBuilder<T> SetItems(IEnumerable<T> items)
    {
        _dg.ItemsSource = items;
        return this;
    }

    public DataGridBuilder<T> AddColumna(
        string header,
        string bindingPath,
        double? width = null)
    {
        _tieneColumnas = true;

        _dg.Columns.Add(
            new DataGridTextColumn
            {
                Header = header,
                Binding = new Binding(bindingPath),
                Width = width.HasValue ? new DataGridLength(width.Value)
                    : new DataGridLength(1, DataGridLengthUnitType.Star)
            }
        );

        return this;
    }

    public void Build()
    {
        if (_contenedor == null)
            throw new InvalidOperationException("Contenedor no definido");

        _dg.AutoGenerateColumns = !_tieneColumnas;

        _contenedor.Children.Add(_dg);
    }
}