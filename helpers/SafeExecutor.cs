using System.Windows;
using Gestion.core.interfaces.service;

namespace Gestion.helpers;

public static class SafeExecutor
{
    public static async Task RunAsync(
        Func<Task> action,
        IDialogService dialogService,
        string mensajeError)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                dialogService.ShowError($"{mensajeError}:\n{ex.Message}");
            });
        }
    }

    public static async Task<T> RunAsync<T>(
            Func<Task<T>> action,
            IDialogService dialogService,
            string mensajeError)
    {
        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                dialogService.ShowError($"{mensajeError}:\n{ex.Message}");
            });

             return default!;
        }
    }
}