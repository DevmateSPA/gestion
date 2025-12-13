using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.presentation.views.windows;

namespace Gestion.core.services;
public class DialogService : IDialogService
{
    public void ShowMessage(string message, string title = "Información")
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public void ShowError(string message, string title = "Error")
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

    public void ShowWarning(string message, string title = "Advertencia")
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);

    public bool Confirm(string message, string title = "Confirmar acción")
        => MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question)
           == MessageBoxResult.Yes;
}