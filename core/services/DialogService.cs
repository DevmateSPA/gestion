using System.Windows;
using Gestion.core.interfaces.service;
using Gestion.presentation.views.windows;

namespace Gestion.core.services;
public class DialogService : IDialogService
{
    private Window? _loadingWindow;
    public void ShowMessage(string message, string title = "Información")
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public void ShowError(string message, string title = "Error")
        => MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

    public void ShowLoading()
    {
        _loadingWindow = new LoadingWindow();
        _loadingWindow.Show();
    }

    public void HideLoading()
    {
        _loadingWindow?.Close();
        _loadingWindow = null;
    } 
}