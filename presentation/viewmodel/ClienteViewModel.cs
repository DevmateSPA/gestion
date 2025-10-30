using System.Windows;

namespace Gestion.presentation.viewmodel;

public class ClienteViewModel : Window
{
    public ClienteViewModel(int numero)
    {
        Title = $"Ventana Modal {numero}";
    }
}
