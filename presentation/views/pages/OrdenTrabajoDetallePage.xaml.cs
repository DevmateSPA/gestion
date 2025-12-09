using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Gestion.core.model;

namespace Gestion.presentation.views.pages;

public partial class OrdenTrabajoDetallePage : Window
{
    private readonly object _entidadOriginal;
    public object EntidadEditada { get; private set; }
    public OrdenTrabajoDetallePage(Page padre, object entidad)
    {
        InitializeComponent();
        this.Owner = Window.GetWindow(padre);

        _entidadOriginal = entidad;

        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }

        this.PreviewKeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                this.DialogResult = false;
            }
        };

        DataContext = entidad;
    }

    private static IEnumerable<T> FindControls<T>(DependencyObject parent) where T : DependencyObject
    {
        if (parent == null)
            yield break;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is T typedChild)
                yield return typedChild;

            foreach (var descendant in FindControls<T>(child))
                yield return descendant;
        }
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        var textBoxes = FindControls<TextBox>(MainGrid);

        foreach (var prop in EntidadEditada.GetType().GetProperties())
        {
            if (!prop.CanWrite)
                continue;

            // Buscar un TextBox con Tag que coincida con el nombre de la propiedad
            var txt = textBoxes.FirstOrDefault(t => t.Tag is string tag && tag == prop.Name);
            
            if (txt != null)
            {
                string texto = txt.Text;

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
                    txt.Focus();
                    return;
                }
            }
            else
            {
                // Si no hay TextBox para esta propiedad, conservar el valor original
                var valorOriginal = _entidadOriginal.GetType().GetProperty(prop.Name)?.GetValue(_entidadOriginal);
                prop.SetValue(EntidadEditada, valorOriginal);
            }
        }

        // Forzar el valor de Empresa al final
        EntidadEditada.GetType().GetProperty("Empresa")?
            .SetValue(EntidadEditada, _entidadOriginal is OrdenTrabajo ot ? ot.Empresa : 0);

        this.DialogResult = true;
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        this.DialogResult = false;
    }
}
