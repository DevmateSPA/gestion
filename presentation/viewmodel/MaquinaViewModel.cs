using System.Windows;

namespace Gestion.presentation.viewmodel;

public class MaquinaViewModel : Window
{
    public MaquinaViewModel(int numero)
    {
        Title = $"Ventana Modal {numero}";
    }
}
