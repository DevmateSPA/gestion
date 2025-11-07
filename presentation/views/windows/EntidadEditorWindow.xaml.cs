using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorWindow : Window
{
    private readonly object _entidadOriginal;
    private readonly Dictionary<PropertyInfo, TextBox> _controles = new();

    public object EntidadEditada { get; private set; }

    public EntidadEditorWindow(object entidad, string titulo = "Ventana")
    {
        InitializeComponent();

        Title = titulo;

        _entidadOriginal = entidad;

        // Clonamos la entidad para no modificar la original hasta confirmar
        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }

        GenerarCampos(EntidadEditada);
    }

    private void GenerarCampos(object entidad)
    {
        var tipo = entidad.GetType();

        var propiedades = tipo.GetProperties()
            .Where(p =>
                p.CanWrite &&
                !string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase) &&
                (p.PropertyType == typeof(string) || p.PropertyType.IsValueType)
            )
            .ToList();

        foreach (var prop in propiedades)
        {
            var label = new TextBlock
            {
                Text = prop.Name,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 8, 0, 2)
            };

            var valorActual = prop.GetValue(entidad)?.ToString() ?? "";
            var textBox = new TextBox
            {
                Text = valorActual,
                Margin = new Thickness(0, 0, 0, 10)
            };

            _controles[prop] = textBox;

            spCampos.Children.Add(label);
            spCampos.Children.Add(textBox);
        }
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
}