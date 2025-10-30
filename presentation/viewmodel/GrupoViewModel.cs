using System.Windows;

namespace Gestion.presentation.viewmodel;

public class GrupoViewModel : Window
{
    public GrupoViewModel(int numero)
    {
        Title = $"Ventana Modal {numero}";
    }
}
