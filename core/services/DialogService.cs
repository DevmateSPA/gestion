using System.Windows;
using Gestion.core.interfaces.service;

namespace Gestion.core.services;
public class DialogService : IDialogService
{
    public void ShowMessage(string message, string title = "Información")
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public void ShowError(string message, string title = "Error")
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
}