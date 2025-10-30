using System.Windows;

namespace Gestion.presentation.viewmodel;

public class ProductoViewModel : Window
{
    public ProductoViewModel(int numero)
    {
        Title = $"Ventana Modal {numero}";
    }
}
