using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Gestion.presentation.views.util.buildersUi;

/// <summary>
/// Builder fluido para la construcción y configuración de un <see cref="DataGrid"/>
/// asociado a una colección de elementos de tipo <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">
/// Tipo de los elementos contenidos en el <see cref="DataGrid"/>.
/// </typeparam>
/// <remarks>
/// <para>
/// Este builder encapsula la configuración más común de un <see cref="DataGrid"/>,
/// permitiendo definir:
/// <list type="bullet">
/// <item>Modo de edición (solo lectura o editable).</item>
/// <item>Origen de datos.</item>
/// <item>Columnas explícitas o autogeneradas.</item>
/// <item>Altura máxima y comportamiento de scroll.</item>
/// </list>
/// </para>
/// <para>
/// Está diseñado para ser utilizado por builders de mayor nivel,
/// como <c>DetallesBuilder</c>, y no directamente desde la vista.
/// </para>
/// </remarks>
public class DataGridBuilder<T>
{
    private readonly DataGrid _dg = new();
    private Panel? _contenedor;
    private bool _tieneColumnas = false;

    /// <summary>
    /// Configura el <see cref="DataGrid"/> en modo solo lectura,
    /// deshabilitando la edición y la manipulación de filas.
    /// </summary>
    public DataGridBuilder<T> SoloLectura()
    {
        _dg.IsReadOnly = true;
        _dg.CanUserAddRows = false;
        _dg.CanUserDeleteRows = false;
        return this;
    }

    /// <summary>
    /// Configura el <see cref="DataGrid"/> en modo editable,
    /// permitiendo agregar y eliminar filas.
    /// </summary>
    public DataGridBuilder<T> Editable()
    {
        _dg.IsReadOnly = false;
        _dg.CanUserAddRows = true;
        _dg.CanUserDeleteRows = true;
        return this;
    }

    /// <summary>
    /// Define el contenedor visual donde se insertará el <see cref="DataGrid"/>.
    /// </summary>
    /// <param name="contenedor">
    /// Panel contenedor (por ejemplo, <see cref="Grid"/> o <see cref="DockPanel"/>).
    /// </param>
    public DataGridBuilder<T> SetContenedor(Panel contenedor)
    {
        _contenedor = contenedor;
        return this;
    }

    /// <summary>
    /// Define la colección de elementos que será utilizada como
    /// origen de datos del <see cref="DataGrid"/>.
    /// </summary>
    /// <param name="items">Colección enumerable de elementos.</param>
    public DataGridBuilder<T> SetItems(IEnumerable items)
    {
        _dg.ItemsSource = items;
        return this;
    }

    /// <summary>
    /// Define la altura máxima del <see cref="DataGrid"/>.
    /// </summary>
    /// <param name="maxHeight">
    /// Altura máxima en unidades de dispositivo (WPF).
    /// </param>
    public DataGridBuilder<T> SetMaxHeight(double maxHeight)
    {
        _dg.MaxHeight = maxHeight;
        return this;
    }

    /// <summary>
    /// Agrega una columna de texto al <see cref="DataGrid"/>.
    /// </summary>
    /// <param name="header">Texto del encabezado de la columna.</param>
    /// <param name="bindingPath">
    /// Ruta de binding hacia la propiedad del elemento de tipo <typeparamref name="T"/>.
    /// </param>
    /// <param name="width">
    /// Ancho fijo opcional de la columna. Si es <c>null</c>, el ancho
    /// se determina automáticamente.
    /// </param>
    /// <param name="fillRemaining">
    /// Indica si la columna debe ocupar el espacio restante disponible
    /// utilizando ancho tipo <c>Star</c>.
    /// </param>
    /// <remarks>
    /// Si al menos una columna es agregada manualmente, la generación
    /// automática de columnas será deshabilitada.
    /// </remarks>
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

    /// <summary>
    /// Finaliza la construcción del <see cref="DataGrid"/> y lo agrega
    /// al contenedor configurado.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Se lanza si el contenedor no ha sido definido o si el contenedor
    /// es un <see cref="StackPanel"/>, el cual no soporta correctamente
    /// el comportamiento de scroll del <see cref="DataGrid"/>.
    /// </exception>
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