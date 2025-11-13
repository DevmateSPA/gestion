using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using Gestion.core.model; // ✅ necesario para IEnumerable<T>

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorTableWindow : Window
{
    private readonly object _entidadOriginal;
    private readonly Dictionary<PropertyInfo, TextBox> _controles = new();
    private readonly IEnumerable<object>? _detalles; // ✅ corregido

    public object EntidadEditada { get; private set; }

    public EntidadEditorTableWindow(Page padre, object entidad, IEnumerable<object>? detalles, string titulo = "Ventana con tabla") // ✅ corregido
    {
        InitializeComponent();
        this.Owner = Window.GetWindow(padre);
        Title = titulo;

        _entidadOriginal = entidad;
        _detalles = detalles;

        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }

        GenerarCampos(EntidadEditada);
        CargarTabla();

        if (_controles.Values.FirstOrDefault() is TextBox primerCampo)
            primerCampo.Focus();

        this.PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        };
    }


    private void GenerarCampos(object entidad)
    {

        var tipo = entidad.GetType();

        var propiedades = tipo.GetProperties()
            .Where(p =>
                p.CanWrite &&
                !string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) &&
                (p.PropertyType == typeof(string) || p.PropertyType.IsValueType) &&
                (p.GetCustomAttribute<VisibleAttribute>()?.Mostrar ?? true)
            )
            .ToList();


        spCampos.Children.Clear();

        int maxPorFila = 3;
        StackPanel filaActual = null;

        for (int i = 0; i < propiedades.Count; i++)
        {
            var prop = propiedades[i];

            var label = new TextBlock
            {
                Text = prop.GetCustomAttribute<NombreAttribute>()?.Texto ?? prop.Name,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 4, 0, 2),
                TextWrapping = TextWrapping.Wrap
            };

            var valorActual = prop.GetValue(entidad)?.ToString() ?? "";
            var textBox = new TextBox
            {
                Text = valorActual,
                FontSize = 20,
                Height = 30,
                Width = 300,
                Margin = new Thickness(5, 0, 5, 10)
            };

            _controles[prop] = textBox;

            var bloque = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Width = 310
            };
            bloque.Children.Add(label);
            bloque.Children.Add(textBox);

            if (i % maxPorFila == 0)
            {
                filaActual = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };
                spCampos.Children.Add(filaActual);
            }

            filaActual.Children.Add(bloque);
        }
    }


    private void CargarTabla()
    {
        if (_detalles == null) return;
        dgDetalles.ItemsSource = _detalles;
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        foreach (var kvp in _controles)
        {
            var prop = kvp.Key;
            var textBox = kvp.Value;
            var texto = textBox.Text;

            try
            {
                object valorConvertido = texto;

                if (prop.PropertyType != typeof(string))
                    valorConvertido = Convert.ChangeType(texto, prop.PropertyType);

                prop.SetValue(EntidadEditada, valorConvertido);
            }
            catch
            {
                MessageBox.Show($"El valor ingresado para {prop.Name} no es válido.");
                return;
            }
        }

        DialogResult = true;
        Close();
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void dgDetalles_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
    {
        if (_entidadOriginal is Factura factura && e.NewItem is Detalle detalle)
            detalle.Folio = factura.Folio;
    }
}
