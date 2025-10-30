using System.Windows;

namespace Gestion.presentation.viewmodel;

public class ProveedorViewModel : Window
{
    public ProveedorViewModel(int numero)
    {
        Title = $"Ventana Modal {numero}";
    }
}
