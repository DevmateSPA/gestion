using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; 
using Gestion.core.model;
using Gestion.presentation.viewmodel;
using Gestion.presentation.views.windows;
using Gestion.presentation.utils;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Media;

namespace Gestion.presentation.views.pages;

public partial class OrdenTrabajoDetallePage : Window
{
    private readonly object _entidadOriginal;
    public object EntidadEditada { get; private set; }
    public OrdenTrabajoDetallePage(Page padre, object entidad)
    {
        InitializeComponent();
        _entidadOriginal = entidad;
        DataContext = _entidadOriginal;

        EntidadEditada = Activator.CreateInstance(entidad.GetType())!;
        foreach (var prop in entidad.GetType().GetProperties())
        {
            if (prop.CanWrite)
                prop.SetValue(EntidadEditada, prop.GetValue(entidad));
        }
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

        foreach (var txt in textBoxes)
        {
            if (txt.Tag is not string propName)
                continue;

            var prop = EntidadEditada.GetType().GetProperty(propName);
            if (prop == null)
                continue;

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
                MessageBox.Show($"El valor ingresado para {propName} no es válido.");
                txt.Focus();
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
