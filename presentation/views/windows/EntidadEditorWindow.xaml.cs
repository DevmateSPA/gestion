using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestion.presentation.utils;

namespace Gestion.presentation.views.windows;

public partial class EntidadEditorWindow : Window
{
    private readonly object _entidadOriginal;
    private readonly Dictionary<PropertyInfo, TextBox> _controles = new();

    public object EntidadEditada { get; private set; }

    public EntidadEditorWindow(Page padre, object entidad, string titulo = "Ventana")
    {
        InitializeComponent();
        this.Owner = Window.GetWindow(padre);
        Title = titulo;

        _entidadOriginal = entidad;

        // Clonar entidad
        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }

        GenerarCampos(EntidadEditada);

        // Cargar MEMO si aplica
        switch (entidad)
        {
            case Gestion.core.model.Factura f:
                spMemo.Visibility = Visibility.Visible;
                txtMemo.Text = f.Memo;
                break;

            case Gestion.core.model.NotaCredito nc:
                spMemo.Visibility = Visibility.Visible;
                txtMemo.Text = nc.Memo;
                break;

            case Gestion.core.model.GuiaDespacho gd:
                spMemo.Visibility = Visibility.Visible;
                txtMemo.Text = gd.Memo;
                break;
        }

        // Focus
        if (_controles.Values.FirstOrDefault() is TextBox primerCampo)
        {
            primerCampo.Focus();
        }

        // ESC para cerrar
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

        // Excluir "Memo" para evitar que aparezca arriba
        var propiedades = tipo.GetProperties()
            .Where(p =>
                p.CanWrite &&
                p.Name != "Memo" &&               // <--- EXCLUIDO
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
                filaActual = new StackPanel { Orientation = Orientation.Horizontal };
                spCampos.Children.Add(filaActual);
            }

            filaActual.Children.Add(bloque);
        }
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        // Guardar propiedades normales
        foreach (var kvp in _controles)
        {
            var prop = kvp.Key;
            var textBox = kvp.Value;
            var texto = textBox.Text;

            try
            {
                // Valida la propiedad segun los metadatos de la entidad
                if (!ValidatorProperties.Validar(prop, texto, out var mensaje))
                {
                    MessageBox.Show(mensaje);
                    textBox.Focus();
                    return;
                }

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

        // Guardar MEMO
        var memoProp = EntidadEditada.GetType().GetProperty("Memo");
        if (memoProp != null)
        {
            memoProp.SetValue(EntidadEditada, txtMemo.Text);
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
