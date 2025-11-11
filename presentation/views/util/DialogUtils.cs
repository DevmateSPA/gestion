using System.Windows;

namespace Gestion.presentation.utils
{
    public static class DialogUtils
    {
        public static bool Confirmar(string mensaje, string titulo = "Confirmar acción")
        {
            var resultado = MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            return resultado == MessageBoxResult.Yes;
        }

        public static void MostrarInfo(string mensaje, string titulo = "Información")
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void MostrarAdvertencia(string mensaje, string titulo = "Advertencia")
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static void MostrarError(string mensaje, string titulo = "Error")
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
