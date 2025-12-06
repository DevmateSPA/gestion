using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorTableWindow : Window
{
    private readonly Dictionary<PropertyInfo, TextBox> _controles = new();
    private ObservableCollection<object> _detallesEditable = new();

    public object EntidadEditada { get; private set; }

    public EntidadEditorTableWindow(Page padre, object entidad, IEnumerable<object>? detalles, string titulo = "Ventana con tabla")
    {
        InitializeComponent();

        Owner = Window.GetWindow(padre);
        Title = titulo;

        EntidadEditada = CopiarEntidad(entidad);
        _detallesEditable = CopiarDetalles(detalles);

        GenerarCampos(EntidadEditada);
        CargarTabla();

        EnfocarPrimerCampo();
        ManejarEscape();
    }

    // --------------------------------------------------------------------
    // COPIAS
    // --------------------------------------------------------------------
    private object CopiarEntidad(object origen)
    {
        var copia = Activator.CreateInstance(origen.GetType())!;
        foreach (var prop in origen.GetType().GetProperties().Where(p => p.CanWrite))
            prop.SetValue(copia, prop.GetValue(origen));

        return copia;
    }

    private ObservableCollection<object> CopiarDetalles(IEnumerable<object>? detalles)
    {
        if (detalles == null)
            return new();

        return new ObservableCollection<object>(
            detalles.Select(d =>
            {
                var copia = Activator.CreateInstance(d.GetType())!;
                foreach (var prop in d.GetType().GetProperties().Where(p => p.CanWrite))
                    prop.SetValue(copia, prop.GetValue(d));
                return copia;
            })
        );
    }

    // --------------------------------------------------------------------
    // CAMPOS
    // --------------------------------------------------------------------
    private void GenerarCampos(object entidad)
    {
        var propiedades = entidad.GetType().GetProperties()
            .Where(p =>
                p.CanWrite &&
                p.PropertyType.IsPublic &&
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

            var bloque = CrearBloqueCampo(prop, prop.GetValue(entidad)?.ToString() ?? "");
            _controles.Add(prop, (TextBox)bloque.Children[1]);

            if (i % maxPorFila == 0)
            {
                filaActual = new StackPanel { Orientation = Orientation.Horizontal };
                spCampos.Children.Add(filaActual);
            }

            filaActual!.Children.Add(bloque);
        }
    }

    private StackPanel CrearBloqueCampo(PropertyInfo prop, string valor)
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Width = 310,
            Children =
            {
                new TextBlock
                {
                    Text = prop.GetCustomAttribute<NombreAttribute>()?.Texto ?? prop.Name,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0,4,0,2),
                    TextWrapping = TextWrapping.Wrap
                },
                new TextBox
                {
                    Text = valor,
                    FontSize = 20,
                    Height = 30,
                    Width = 300,
                    Margin = new Thickness(5,0,5,10)
                }
            }
        };
    }

    // --------------------------------------------------------------------
    // TABLA
    // --------------------------------------------------------------------
    private void CargarTabla()
    {
        dgDetalles.ItemsSource = _detallesEditable;
    }

    // --------------------------------------------------------------------
    // GUARDAR / CANCELAR
    // --------------------------------------------------------------------
    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        if (!GuardarCamposBasicos())
            return;

        GuardarDetallesEnEntidad();

        DialogResult = true;
        Close();
    }

    private bool GuardarCamposBasicos()
    {
        foreach (var kvp in _controles)
        {
            var prop = kvp.Key;
            var texto = kvp.Value.Text;

            try
            {
                var valor = prop.PropertyType == typeof(string)
                    ? texto
                    : Convert.ChangeType(texto, prop.PropertyType);

                prop.SetValue(EntidadEditada, valor);
            }
            catch
            {
                MessageBox.Show($"El valor ingresado para '{prop.Name}' no es válido.");
                return false;
            }
        }

        return true;
    }

    private void GuardarDetallesEnEntidad()
    {
        var propDetalles = EntidadEditada
            .GetType()
            .GetProperties()
            .FirstOrDefault(p => typeof(IEnumerable<object>).IsAssignableFrom(p.PropertyType));

        if (propDetalles == null)
            return;

        var tipoColeccion = propDetalles.PropertyType;
        var coleccionFinal = Activator.CreateInstance(tipoColeccion);
        var metodoAdd = tipoColeccion.GetMethod("Add");

        foreach (var item in _detallesEditable)
            metodoAdd!.Invoke(coleccionFinal, new[] { item });

        propDetalles.SetValue(EntidadEditada, coleccionFinal);
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    // --------------------------------------------------------------------
    // UX
    // --------------------------------------------------------------------
    private void EnfocarPrimerCampo()
    {
        if (_controles.Values.FirstOrDefault() is TextBox primerCampo)
            primerCampo.Focus();
    }

    private void ManejarEscape()
    {
        PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
        };
    }
}