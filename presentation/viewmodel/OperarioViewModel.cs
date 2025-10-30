using System.Windows;

namespace Gestion.presentation.viewmodel;

public class OperarioViewModel : Window
{
    public OperarioViewModel(int numero)
    {
        Title = $"Ventana Modal {numero}";
    }
}
