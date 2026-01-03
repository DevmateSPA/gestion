using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Gestion.presentation.views.util.buildersUi;

public static class LabelFactory
{
    public static TextBlock Crear(PropertyInfo prop)
    {
        return new TextBlock
        {
            Text = ObtenerTexto(prop),
            FontSize = 16,
            FontWeight = FontWeights.Bold,
            Margin = new Thickness(0, 4, 175, 4),
            TextWrapping = TextWrapping.Wrap
        };
    }

    private static string ObtenerTexto(PropertyInfo prop)
    {
        return prop.GetCustomAttribute<NombreAttribute>()?.Texto
               ?? prop.Name;
    }
}