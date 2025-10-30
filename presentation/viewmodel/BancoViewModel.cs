using System.Windows;

namespace Gestion.presentation.viewmodel;

public class BancoViewModel : Window
{
    public BancoViewModel(int numero)
    {
        Title = $"Ventana Modal {numero}";
    }
}
